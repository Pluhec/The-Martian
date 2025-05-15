using UnityEngine;
using UnityEngine.UI;

/// Jeden skript pro bednu – umí ji sebrat *a* otevřít / zavřít
/// (sebrat = přesun do DontDestroyOnLoad + ikona v inventáři).
public class ContainerItem : InteractableObject
{
    [Header("Prefab ikony bedny do inventáře (má ItemButton)")]
    public GameObject itemButtonPrefab;

    [Header("Počet slotů, které ikona zabírá")]
    public int slotSize = 2;

    StorageContainer box;
    Inventory        inventory;
    PersistentItem   pItem;

    void Awake()
    {
        box       = GetComponent<StorageContainer>();
        inventory = Inventory.Instance;
        pItem     = GetComponent<PersistentItem>();

        actions.Add("sebrat");
        actions.Add("otevřít");
    }

    public override void PerformAction(string action)
    {
        switch (action)
        {
            case "otevřít":  box?.Open();  break;
            case "sebrat":   TryPickUp();  break;
        }
    }

    /*──────────────────────────────*/
    /*  PICKUP LOGIKA                */
    /*──────────────────────────────*/
    void TryPickUp()
    {
        if (inventory == null || itemButtonPrefab == null) return;

        // 1) hledání volného bloku v inventáři
        for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
        {
            bool free = true;
            for (int j = 0; j < slotSize; j++)
                if (inventory.isFull[i + j]) { free = false; break; }
            if (!free) continue;

            // 2) hlavní ikona
            GameObject main = Instantiate(itemButtonPrefab,
                inventory.slots[i].transform, false);

            var btn = main.GetComponent<ItemButton>();
            if (btn == null) { Destroy(main); return; }

            btn.itemID        = pItem.itemID;   // ID bedny = ID ikony
            btn.mainSlotIndex = i;
            btn.slotSize      = slotSize;

            /* IKONA UŽ MÁ ContainerSpawn ► jen ho nakonfigurujeme */
            var csp = main.GetComponent<ContainerSpawn>();
            if (csp == null)
            {
                Debug.LogError("[ContainerItem] Prefab ikony musí mít ContainerSpawn!");
                Destroy(main);
                return;
            }
            csp.containerID = pItem.itemID;
            csp.iconButton  = btn;                 // aby SpawnContainer mohl volat RemoveItem

            inventory.isFull[i] = true;

            // 3) placeholdery
            for (int j = 1; j < slotSize; j++)
            {
                int idx = i + j;
                var ph = Instantiate(itemButtonPrefab,
                                     inventory.slots[idx].transform, false);

                Destroy(ph.GetComponent<ItemButton>());
                Destroy(ph.GetComponent<Button>());

                var img = ph.GetComponent<Image>();
                if (img != null)
                {
                    img.color         = new Color(1,1,1,0.35f);
                    img.raycastTarget = false;
                }
                ph.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                inventory.isFull[idx] = true;
            }

            // 4) perzistence + úklid
            pItem.MarkCollected();
            DroppedItemManager.Instance?.RemoveDroppedItem(gameObject);

            // 5) přesun do DontDestroy a deaktivace
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            ContainerRepository.Register(pItem.itemID, gameObject);
            break;      // hotovo
        }
    }
}
