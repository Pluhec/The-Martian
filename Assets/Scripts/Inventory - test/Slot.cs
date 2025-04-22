using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [Header("Index tohoto slotu v poli")]
    public int i;

    [Header("Je to slot INVENTÁŘE? Jinak je to slot KONTEJNERU")]
    public bool IsInventorySlot = true;

    [Tooltip("Přiřaď jen pokud je to slot kontejneru; jinak se najde automaticky")]
    public StorageContainer container;

    private Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        if (!IsInventorySlot && container == null)
        {
            // Automaticky najdeme kontejner ve vyšší úrovni
            container = GetComponentInParent<StorageContainer>();
            if (container == null)
                Debug.LogError($"[Slot] i={i}: nenašel jsem StorageContainer pro kontejnerový slot!");
        }

        // Kontrola indexu vs. délka polí
        if (IsInventorySlot)
        {
            if (i < 0 || i >= inventory.isFull.Length)
                Debug.LogError($"[Slot] i={i}: mimo rozsah inventory.isFull (délka {inventory.isFull.Length})!");
        }
        else
        {
            if (container != null && (i < 0 || i >= container.isFull.Length))
                Debug.LogError($"[Slot] i={i}: mimo rozsah container.isFull (délka {container.isFull.Length})!");
        }
    }

    void Update()
    {
        // Uvolníme isFull, pokud slot prázdný
        if (transform.childCount <= 0)
        {
            if (IsInventorySlot)
            {
                if (i >= 0 && i < inventory.isFull.Length)
                    inventory.isFull[i] = false;
            }
            else if (container != null)
            {
                if (i >= 0 && i < container.isFull.Length)
                    container.isFull[i] = false;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        if (dropped == null) return;

        var item = dropped.GetComponent<ItemButton>();
        if (item == null) return;

        int size = item.slotSize;
        int originalIndex = item.mainSlotIndex;
        bool placed = false;

        if (IsInventorySlot)
        {
            // 1) Zkus přidat do inventáře
            placed = inventory.AddItem(dropped.gameObject, size);

            // 2) Pokud uspěješ, smaž ze starého úložiště (kontejneru)
            if (placed)
                item.storageContainer?.RemoveItem(originalIndex, size);
        }
        else
        {
            placed = container.AddItem(dropped.gameObject, size);
            if (placed)
                item.inventory?.RemoveItem(originalIndex, size);
        }

        // 3) Pokud se nepodařilo umístit, vrať starý stav
        if (!placed)
        {
            item.inventory?.AlignItems();
            item.storageContainer?.AlignItems();
        }
    }


}
