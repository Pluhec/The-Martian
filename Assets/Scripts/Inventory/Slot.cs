using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public int i;
    public bool IsInventorySlot = true;
    private StorageContainer container;

    Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        if (!IsInventorySlot && container == null)
            container = GetComponentInParent<StorageContainer>();
    }

    public void OnDrop(PointerEventData e)
    {
        if (!ItemButton.isDragEnabled) return;

        var go = e.pointerDrag;
        if (go == null) return;

        var item = go.GetComponent<ItemButton>();
        if (item == null) return;

        int size = item.slotSize;
        int src = item.mainSlotIndex;

        int targetIndex = i;
        bool ok = false;

        if (IsInventorySlot)
        {
            if (targetIndex + size > inventory.slots.Length)
                targetIndex = inventory.slots.Length - size;

            ok = inventory.AddItemAt(targetIndex, go, size);
            if (ok)
            {
                item.storageContainer?.VacateSlots(src, size, go);
                item.inventory = inventory;
                item.storageContainer = null;
            }
        }
        else
        {
            if (targetIndex + size > container.slots.Length)
                targetIndex = container.slots.Length - size;

            ok = container.AddItemAt(targetIndex, go, size);
            if (ok)
            {
                item.inventory?.VacateSlots(src, size, go);
                item.storageContainer = container;
                item.inventory = null;
            }
        }

        if (!ok)
        {
            if (IsInventorySlot) inventory.VacateSlots(i, size, null);
            else container.VacateSlots(i, size, null);

            item.inventory?.AlignItems();
            item.storageContainer?.AlignItems();
        }
    }

    public void OnDirectAdd(GameObject icon, int size)
    {
        icon.transform.SetParent(transform, false);
        var btn = icon.GetComponent<ItemButton>();
        btn.mainSlotIndex = i;
        btn.slotSize = size;
    }
}