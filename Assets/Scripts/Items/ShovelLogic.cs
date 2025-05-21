using UnityEngine;
using UnityEngine.Tilemaps;

public static class ShovelLogic
{
    private static GameObject dirtItemPrefab;
    private static TileBase dugTile;

    public static void Initialize(GameObject dirtPrefab, TileBase tile)
    {
        dirtItemPrefab = dirtPrefab;
        dugTile = tile;
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
            Debug.Log("[ShovelLogic] Není místo v inventáři.");
            return;
        }

        // Kontrola typu dlaždice
        Vector3Int cell = tilemap.WorldToCell(player.position);
        TileBase currentTile = tilemap.GetTile(cell);
        
        if (currentTile == null)
        {
            Debug.Log("[ShovelLogic] Na této pozici není žádná dlaždice.");
            return;
        }
        
        // Kontrola, jestli je to písek
        if (!currentTile.name.ToLower().Contains("sand"))
        {
            Debug.Log("[ShovelLogic] Zde není písek, nelze kopat.");
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