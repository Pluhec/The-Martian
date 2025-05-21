using UnityEngine;
using UnityEngine.UI;

public class ContainerItem : InteractableObject
{
    [Header("Prefab ikony bedny do inventáře (má ItemButton)")]
    public GameObject itemButtonPrefab;

    [Header("Počet slotů, které ikona zabírá")]
    public int slotSize = 2;

    StorageContainer box;
    Inventory        inventory;
    PersistentItem   pItem;
    private GameObject toastPrefab;
    private Transform notificationsParent;

    void Awake()
    {
        box = GetComponent<StorageContainer>();
        inventory = Inventory.Instance;
        pItem = GetComponent<PersistentItem>();

        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }

        actions.Add("Pick Up");
        actions.Add("Open");
    }

    public override void PerformAction(string action)
    {
        switch (action)
        {
            case "Open":  box?.Open();  break;
            case "Pick Up":   TryPickUp();  break;
        }
    }

    void TryPickUp()
    {
        if (inventory == null || itemButtonPrefab == null) return;

        bool foundSpace = false;
        for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
        {
            bool free = true;
            for (int j = 0; j < slotSize; j++)
                if (inventory.isFull[i + j]) { free = false; break; }
            if (!free) continue;

            foundSpace = true;
            GameObject main = Instantiate(itemButtonPrefab, inventory.slots[i].transform, false);

            var btn = main.GetComponent<ItemButton>();
            if (btn == null) { Destroy(main); return; }

            btn.itemID = pItem.itemID;
            btn.mainSlotIndex = i;
            btn.slotSize = slotSize;

            var csp = main.GetComponent<ContainerSpawn>();
            if (csp == null)
            {
                Debug.LogError("[ContainerItem] Prefab ikony musí mít ContainerSpawn!");
                Destroy(main);
                return;
            }
            csp.containerID = pItem.itemID;
            csp.iconButton = btn;

            inventory.isFull[i] = true;

            for (int j = 1; j < slotSize; j++)
            {
                int idx = i + j;
                var ph = Instantiate(itemButtonPrefab, inventory.slots[idx].transform, false);

                Destroy(ph.GetComponent<ItemButton>());
                Destroy(ph.GetComponent<Button>());

                var img = ph.GetComponent<Image>();
                if (img != null)
                {
                    img.color = new Color(1,1,1,0.35f);
                    img.raycastTarget = false;
                }
                ph.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                inventory.isFull[idx] = true;
            }

            pItem.MarkCollected();
            DroppedItemManager.Instance?.RemoveDroppedItem(gameObject);
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            ContainerRepository.Register(pItem.itemID, gameObject);
            break;
        }

        if (!foundSpace && toastPrefab != null && notificationsParent != null)
        {
            var toast = Instantiate(toastPrefab, notificationsParent);
            toast.GetComponent<Toast>()?.Show("warning", "Not enough space in inventory!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) box?.Close();
    }
}