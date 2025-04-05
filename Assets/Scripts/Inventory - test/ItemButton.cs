using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    public int mainSlotIndex;
    public int slotSize = 3;
    private Inventory inventory;

    private void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Levý klik!");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Pravý klik!");
            DropItem();
        }
    }

    private void DropItem()
    {
        for (int i = 0; i < slotSize; i++)
        {
            int index = mainSlotIndex + i;
            Transform slot = inventory.slots[index].transform;

            if (slot.childCount > 0)
            {
                GameObject child = slot.GetChild(0).gameObject;

                // Spawn se dělá jen u hlavního itemu
                if (i == 0 && child.TryGetComponent<Spawn>(out var spawn))
                {
                    spawn.SpawnDroppedItem();
                }

                Destroy(child);
                inventory.isFull[index] = false;
            }
        }
    }
}