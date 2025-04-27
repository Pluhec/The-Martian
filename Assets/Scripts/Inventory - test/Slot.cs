using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public int  i;
    public bool IsInventorySlot = true;
    public StorageContainer container;

    Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        if (!IsInventorySlot && container == null)
            container = GetComponentInParent<StorageContainer>();
    }

    public void OnDrop(PointerEventData e)
    {
        var go = e.pointerDrag;
        if (go == null) return;

        var item = go.GetComponent<ItemButton>();
        if (item == null) return;

        int size = item.slotSize;
        int src  = item.mainSlotIndex;
        bool ok;

        if (IsInventorySlot)
        {
            ok = inventory.AddItemAt(i, go, size);
            if (ok)
            {
                item.storageContainer?.VacateSlots(src, size, go);
                item.inventory        = inventory;
                item.storageContainer = null;
            }
        }
        else
        {
            ok = container.AddItemAt(i, go, size);
            if (ok)
            {
                item.inventory?.VacateSlots(src, size, go);
                item.storageContainer = container;
                item.inventory        = null;
            }
        }

        if (!ok)
        {
            if (IsInventorySlot) inventory.VacateSlots(i, size, null);
            else                 container.VacateSlots(i, size, null);

            item.inventory?.AlignItems();
            item.storageContainer?.AlignItems();
        }
    }
}