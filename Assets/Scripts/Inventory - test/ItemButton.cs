using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    public int mainSlotIndex;
    public int slotSize = 2;
    private Inventory inventory;

    private void Awake()
    {
        // Bezpečné získání inventáře
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance not found!");
            return;
        }
        inventory = Inventory.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        // Kontrola existence závislostí
        if (inventory == null || inventory.slots == null) return;
        if (DroppedItemManager.Instance == null) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 dropPosition = new Vector2(player.position.x, player.position.y + 0.35f);

        for (int i = 0; i < slotSize; i++)
        {
            int index = mainSlotIndex + i;
            
            // Kontrola platnosti indexu
            if (index >= inventory.slots.Length) break;

            Transform slot = inventory.slots[index].transform;
            if (slot.childCount == 0) continue;

            GameObject child = slot.GetChild(0).gameObject;
            
            // Hlavní item spawnuje fyzický objekt
            if (i == 0 && child.TryGetComponent<Spawn>(out var spawn))
            {
                GameObject spawnedItem = Instantiate(spawn.item, dropPosition, Quaternion.identity);
                DroppedItemManager.Instance.AddDroppedItem(spawn.item, dropPosition);
            }

            Destroy(child);
            inventory.isFull[index] = false;
        }
    }
}