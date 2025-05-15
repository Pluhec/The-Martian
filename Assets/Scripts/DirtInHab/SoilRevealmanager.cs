using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class SoilRevealManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap visibleTilemap;
    public Tilemap completeTilemap;

    [Header("Settings")]
    public int maxSoilAmount = 30;
    public bool autoUpdate = true;

    [Header("Debug Control")]
    [Range(0, 30)]
    public int currentSoilAmount = 0;

    private int lastSoilAmount = -1;

    private List<Vector3Int> allTilePositions = new List<Vector3Int>();
    private HashSet<Vector3Int> revealedPositions = new HashSet<Vector3Int>();

    private void Start()
    {
        Debug.Log("SoilRevealManager: Start() zavolán");
        
        if (visibleTilemap == null)
        {
            Debug.LogError("SoilRevealManager: visibleTilemap reference chybí!");
        }
        
        if (completeTilemap == null)
        {
            Debug.LogError("SoilRevealManager: completeTilemap reference chybí!");
        }
        
        visibleTilemap.ClearAllTiles();
        LoadTilePositions();
        UpdateRevealedTiles();
        lastSoilAmount = currentSoilAmount;
        Debug.Log($"SoilRevealManager: Inicializace dokončena, currentSoilAmount = {currentSoilAmount}");
    }

    private void Update()
    {
        if (autoUpdate && lastSoilAmount != currentSoilAmount)
        {
            Debug.Log($"SoilRevealManager: Detekována změna množství půdy v Update: {lastSoilAmount} -> {currentSoilAmount}");
            UpdateRevealedTiles();
            lastSoilAmount = currentSoilAmount;
        }
    }

    // na debug
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // nacitalo se to pred scriptama, nikdo nevi proc..
        if (!enabled || !gameObject.activeInHierarchy || visibleTilemap == null || completeTilemap == null)
        {
            Debug.LogWarning("SoilRevealManager: OnValidate přeskočen, komponenty nejsou připraveny");
            return;
        }

        if (lastSoilAmount != currentSoilAmount)
        {
            Debug.Log($"SoilRevealManager: OnValidate - detekována změna množství půdy: {lastSoilAmount} -> {currentSoilAmount}");
            if (allTilePositions.Count == 0)
            {
                LoadTilePositions();
            }
            UpdateRevealedTiles();
            lastSoilAmount = currentSoilAmount;
        }
    }
    #endif

    private void LoadTilePositions()
    {
        Debug.Log("SoilRevealManager: Načítám pozice dlaždiček");
        allTilePositions.Clear();

        // hranice
        BoundsInt bounds = completeTilemap.cellBounds;
        Debug.Log($"SoilRevealManager: Hranice tilemapy: {bounds}");

        // nacteni hrnic
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (completeTilemap.HasTile(pos))
            {
                allTilePositions.Add(pos);
            }
        }
        
        SortPositionsByDistanceFromCenter();

        Debug.Log($"SoilRevealManager: Načteno {allTilePositions.Count} pozic dlaždic z complete tilemap.");
    }
    
    private void SortPositionsByDistanceFromCenter()
    {
        Vector3 center = CalculateCenterOfTilemap();
        Debug.Log($"SoilRevealManager: Střed tilemapy: {center}");

        // od vzdalenosti od stredu
        allTilePositions = allTilePositions
            .OrderBy(pos => Vector3.Distance(completeTilemap.CellToWorld(pos), center))
            .ToList();
            
        Debug.Log("SoilRevealManager: Pozice seřazeny podle vzdálenosti od středu");
    }
    
    private Vector3 CalculateCenterOfTilemap()
    {
        if (allTilePositions.Count == 0)
        {
            Debug.LogWarning("SoilRevealManager: Nelze vypočítat střed tilemapy, žádné pozice nejsou k dispozici");
            return Vector3.zero;
        }
        
        Vector3 sum = Vector2.zero;
        foreach (Vector3Int pos in allTilePositions)
        {
            sum += completeTilemap.CellToWorld(pos);
        }

        return sum / allTilePositions.Count;
    }
    
    public void UpdateRevealedTiles()
    {
        Debug.Log($"SoilRevealManager: UpdateRevealedTiles() zavolán, currentSoilAmount = {currentSoilAmount}");
        
        // Kontrola, zda jsou data připravena
        if (allTilePositions.Count == 0)
        {
            Debug.LogWarning("SoilRevealManager: allTilePositions je prázdný, načítám pozice");
            LoadTilePositions();
        }

        int numTilesToReveal = CalculateTilesToReveal();
        Debug.Log($"SoilRevealManager: Počet dlaždic k odhalení: {numTilesToReveal} z celkových {allTilePositions.Count}");
        
        HashSet<Vector3Int> newRevealedPositions = new HashSet<Vector3Int>();

        // pridava prislunsy pocet novych tilu
        for (int i = 0; i < numTilesToReveal && i < allTilePositions.Count; i++)
        {
            newRevealedPositions.Add(allTilePositions[i]);
        }

        HashSet<Vector3Int> positionsToReveal = new HashSet<Vector3Int>(newRevealedPositions);
        positionsToReveal.ExceptWith(revealedPositions);

        HashSet<Vector3Int> positionsToHide = new HashSet<Vector3Int>(revealedPositions);
        positionsToHide.ExceptWith(newRevealedPositions);
        
        Debug.Log($"SoilRevealManager: Odhalování {positionsToReveal.Count} nových dlaždic, skrývání {positionsToHide.Count} dlaždic");

        // Tady pomahal chat, protoze se mi nechteli nacitat spravne narotovane tily
        foreach (Vector3Int pos in positionsToReveal)
        {
            TileBase tile = completeTilemap.GetTile(pos);
            if (tile == null)
            {
                Debug.LogError($"SoilRevealManager: Na pozici {pos} nebyla nalezena dlaždice v completeTilemap!");
                continue;
            }
            
            Matrix4x4 transformMatrix = completeTilemap.GetTransformMatrix(pos);
            visibleTilemap.SetTile(pos, tile);
            visibleTilemap.SetTransformMatrix(pos, transformMatrix);
        }

        foreach (Vector3Int pos in positionsToHide)
        {
            visibleTilemap.SetTile(pos, null);
        }

        revealedPositions = newRevealedPositions;
        Debug.Log($"SoilRevealManager: Aktualizace dlaždic dokončena, celkem odhaleno: {revealedPositions.Count}");
    }

    // kolik se jich ma zobrazit
    private int CalculateTilesToReveal()
    {
        float percentage = Mathf.Clamp01((float)currentSoilAmount / maxSoilAmount);
        int result = Mathf.FloorToInt(percentage * allTilePositions.Count);
        Debug.Log($"SoilRevealManager: Vypočítáno {result} dlaždic k odhalení (percentage = {percentage:F2})");
        return result;
    }

    // toto se bude volat, kdyz dalsi script uciti dopadnuti hliny 
    public void AddSoil(int amount)
    {
        Debug.Log($"SoilRevealManager: AddSoil({amount}) zavolán, aktuální hodnota: {currentSoilAmount}");
        int previousAmount = currentSoilAmount;
        currentSoilAmount = Mathf.Clamp(currentSoilAmount + amount, 0, maxSoilAmount);
        Debug.Log($"SoilRevealManager: Hodnota půdy změněna: {previousAmount} -> {currentSoilAmount}");
        
        UpdateRevealedTiles();
        lastSoilAmount = currentSoilAmount;
    }
    
    public void Reset()
    {
        Debug.Log("SoilRevealManager: Reset() zavolán");
        currentSoilAmount = 0;
        visibleTilemap.ClearAllTiles();
        revealedPositions.Clear();
        lastSoilAmount = -1;
        UpdateRevealedTiles();
        Debug.Log("SoilRevealManager: Reset dokončen");
    }
}

