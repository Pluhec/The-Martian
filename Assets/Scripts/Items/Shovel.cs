using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Shovel : InteractableObject
{
    private Tilemap tilemap;
    public TileBase dugTile;
    public GameObject dirtItemPrefab;
    private GameObject toastPrefab;
    private Transform notificationsParent;

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
        ShovelLogic.Initialize(dirtItemPrefab, dugTile);

        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }

        actions.Add("Dig");
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

            bool foundSpace = false;
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

                foundSpace = true;
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
                gameObject.SetActive(false);
                return;
            }

            if (!foundSpace && toastPrefab != null && notificationsParent != null)
            {
                var toast = Instantiate(toastPrefab, notificationsParent);
                toast.GetComponent<Toast>()?.Show("warning", "Not enough space in inventory!");
            }
        }

        if (action == "Dig")
        {
            if (tilemap == null)
            {
                ShowToast("warning", "Cannot dig here!");
                return;
            }

            if (!HasFreeInventorySlot())
            {
                ShowToast("warning", "Not enough space for dirt!");
                return;
            }

            Vector3Int cellPosition = tilemap.WorldToCell(player.position);
            TileBase currentTile = tilemap.GetTile(cellPosition);
            
            if (currentTile == null || currentTile == dugTile)
            {
                ShowToast("warning", "Cannot dig this type of ground!");
                return;
            }

            ShovelLogic.Dig();
        }
    }

    private void ShowToast(string type, string message)
    {
        if (toastPrefab != null && notificationsParent != null)
        {
            var toast = Instantiate(toastPrefab, notificationsParent);
            toast.GetComponent<Toast>()?.Show(type, message);
        }
    }
}