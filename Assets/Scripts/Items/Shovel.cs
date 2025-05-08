using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Shovel : InteractableObject
{
    private Tilemap tilemap;
    public TileBase dugTile;
    public GameObject dirtItemPrefab;

    private Transform player;
    private Inventory inventory;
    public GameObject itemButton;
    public int slotSize = 2;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("[Shovel] Player tag not found in scene.");

        tilemap = FindObjectOfType<Tilemap>();
        if (tilemap == null)
            Debug.LogError("[Shovel] Tilemap component not found in scene.");

        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance missing!");
            return;
        }
        inventory = Inventory.Instance;

        actions.Add("Use");
        actions.Add("Pick Up");
    }

    private bool HasFreeInventorySlot()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (!inventory.isFull[i]) return true;
        }
        return false;
    }

    public override void PerformAction(string action)
    {
        if (action == "Pick Up")
        {
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
                if (itemScript != null)
                {
                    itemScript.mainSlotIndex = i;
                    itemScript.slotSize = slotSize;
                    itemScript.sourceObject = this;
                    itemScript.inventory = inventory;
                }

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
                DroppedItemManager.Instance?.RemoveDroppedItem(gameObject);

                gameObject.SetActive(false); // Don't destroy, just deactivate
                return;
            }
        }
        else if (action == "Use")
        {
            if (!HasFreeInventorySlot())
            {
                Debug.Log("[Shovel] Není místo v inventáři pro hlínu.");
                return;
            }

            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            tilemap = FindObjectOfType<Tilemap>();

            if (player == null || tilemap == null)
            {
                Debug.Log("[Shovel] Nelze kopat z inventáře.");
                return;
            }

            Vector3Int cell = tilemap.WorldToCell(player.position);
            if (tilemap.GetTile(cell) != null)
            {
                tilemap.SetTile(cell, dugTile);

                for (int i = 0; i < inventory.slots.Length; i++)
                {
                    if (inventory.isFull[i]) continue;

                    GameObject btn = Instantiate(dirtItemPrefab, inventory.slots[i].transform, false);
                    inventory.isFull[i] = true;

                    ItemButton ib = btn.GetComponent<ItemButton>();
                    if (ib != null)
                    {
                        ib.mainSlotIndex = i;
                        ib.slotSize = 1;
                        ib.inventory = inventory;
                        ib.sourceObject = null;
                    }
                    break;
                }
            }
        }
    }
}