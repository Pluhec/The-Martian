using UnityEngine;
using UnityEngine.Tilemaps;

public static class ShovelLogic
{
    private static GameObject dirtItemPrefab;
    private static TileBase dugTile;
    private static GameObject toastPrefab;
    private static Transform notificationsParent;

    public static void Initialize(GameObject dirtPrefab, TileBase tile)
    {
        dirtItemPrefab = dirtPrefab;
        dugTile = tile;

        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }
    }

    private static void ShowToast(string type, string message)
    {
        if (toastPrefab != null && notificationsParent != null)
        {
            var toast = Object.Instantiate(toastPrefab, notificationsParent);
            toast.GetComponent<Toast>()?.Show(type, message);
        }
    }

    public static void Dig()
    {
        if (dirtItemPrefab == null || dugTile == null)
        {
            Debug.LogError("[ShovelLogic] Není inicializován!");
            return;
        }

        Inventory inventory = Inventory.Instance;
        Tilemap tilemap = Object.FindObjectOfType<Tilemap>();
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (inventory == null || tilemap == null || player == null)
        {
            Debug.LogError("[ShovelLogic] Chybí potřebné komponenty");
            ShowToast("warning", "Cannot dig here!");
            return;
        }

        // Kontrola místa v inventáři
        bool hasSpace = false;
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (!inventory.isFull[i])
            {
                hasSpace = true;
                break;
            }
        }

        if (!hasSpace)
        {
            ShowToast("warning", "Not enough space for dirt!");
            return;
        }

        // Kontrola typu dlaždice
        Vector3Int cell = tilemap.WorldToCell(player.position);
        TileBase currentTile = tilemap.GetTile(cell);
        
        if (currentTile == null || currentTile == dugTile)
        {
            ShowToast("warning", "Cannot dig this type of ground!");
            return;
        }
        
        // Kontrola, jestli je to písek
        if (!currentTile.name.ToLower().Contains("sand"))
        {
            ShowToast("warning", "Cannot dig this type of ground!");
            return;
        }

        // Kopání
        tilemap.SetTile(cell, dugTile);

        // Přidání hlíny do inventáře
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i]) continue;

            GameObject btn = Object.Instantiate(dirtItemPrefab, inventory.slots[i].transform, false);
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