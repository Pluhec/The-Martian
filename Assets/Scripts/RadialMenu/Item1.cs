using UnityEngine;
using UnityEngine.UI;

public class Item1 : InteractableObject
{
    private Inventory inventory;
    public GameObject itemButton;
    public int slotSize = 2;

    void Awake()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance missing!");
            return;
        }
        inventory = Inventory.Instance;

        actions.Add("sebrat");
        actions.Add("nasednout");
        actions.Add("opravit");
    }

    public override void PerformAction(string action)
    {
        if (action != "sebrat") return;
        if (inventory.slots == null) return;

        for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
        {
            bool isSpaceFree = true;

            for (int j = 0; j < slotSize; j++)
            {
                if (i + j >= inventory.slots.Length || inventory.isFull[i + j])
                {
                    isSpaceFree = false;
                    break;
                }
            }

            if (!isSpaceFree) continue;

            GameObject mainItem = Instantiate(itemButton, inventory.slots[i].transform, false);
            inventory.isFull[i] = true;

            ItemButton itemScript = mainItem.GetComponent<ItemButton>();
            if (itemScript == null) continue;

            itemScript.mainSlotIndex = i;
            itemScript.slotSize = slotSize;

            for (int j = 1; j < slotSize; j++)
            {
                int targetIndex = i + j;
                if (targetIndex >= inventory.slots.Length) break;

                GameObject placeholder = Instantiate(itemButton, inventory.slots[targetIndex].transform, false);

                Destroy(placeholder.GetComponent<ItemButton>());
                Destroy(placeholder.GetComponent<Button>());

                Image img = placeholder.GetComponent<Image>();
                if (img != null) img.color = new Color(1, 1, 1, 0.35f);

                placeholder.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                inventory.isFull[targetIndex] = true;
            }

            GetComponent<PersistentItem>()?.MarkCollected();

            if (DroppedItemManager.Instance != null)
                DroppedItemManager.Instance.RemoveDroppedItem(gameObject);

            Destroy(gameObject);
            break;
        }
    }
}