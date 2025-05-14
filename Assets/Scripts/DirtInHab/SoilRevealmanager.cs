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
        visibleTilemap.ClearAllTiles();
        LoadTilePositions();
        UpdateRevealedTiles();
        lastSoilAmount = currentSoilAmount;
    }

    private void Update()
    {
        if (autoUpdate && lastSoilAmount != currentSoilAmount)
        {
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
            return;

        if (lastSoilAmount != currentSoilAmount)
        {
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
        allTilePositions.Clear();

        // hranice
        BoundsInt bounds = completeTilemap.cellBounds;

        // nacteni hrnic
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (completeTilemap.HasTile(pos))
            {
                allTilePositions.Add(pos);
            }
        }
        
        SortPositionsByDistanceFromCenter();

        Debug.Log($"Loaded {allTilePositions.Count} tile positions from complete tilemap.");
    }
    
    private void SortPositionsByDistanceFromCenter()
    {
        Vector3 center = CalculateCenterOfTilemap();

        // od vzdalenosti od stredu
        allTilePositions = allTilePositions
            .OrderBy(pos => Vector3.Distance(completeTilemap.CellToWorld(pos), center))
            .ToList();
    }
    
    private Vector3 CalculateCenterOfTilemap()
    {
        if (allTilePositions.Count == 0)
            return Vector3.zero;
        
        Vector3 sum = Vector2.zero;
        foreach (Vector3Int pos in allTilePositions)
        {
            sum += completeTilemap.CellToWorld(pos);
        }

        return sum / allTilePositions.Count;
    }
    
    public void UpdateRevealedTiles()
    {
        // Kontrola, zda jsou data připravena
        if (allTilePositions.Count == 0)
        {
            LoadTilePositions();
        }

        int numTilesToReveal = CalculateTilesToReveal();
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

        // Tady pomahal chat, protoze se mi nechteli nacitat spravne narotovane tily
        foreach (Vector3Int pos in positionsToReveal)
        {
            TileBase tile = completeTilemap.GetTile(pos);
            Matrix4x4 transformMatrix = completeTilemap.GetTransformMatrix(pos);
            visibleTilemap.SetTile(pos, tile);
            visibleTilemap.SetTransformMatrix(pos, transformMatrix);
        }

        foreach (Vector3Int pos in positionsToHide)
        {
            visibleTilemap.SetTile(pos, null);
        }

        revealedPositions = newRevealedPositions;
    }

    // kolik se jich ma zobrazit
    private int CalculateTilesToReveal()
    {
        float percentage = Mathf.Clamp01((float)currentSoilAmount / maxSoilAmount);
        return Mathf.FloorToInt(percentage * allTilePositions.Count);
    }

    // toto se bude volat, kdyz dalsi script uciti dopadnuti hliny 
    public void AddSoil(int amount)
    {
        currentSoilAmount = Mathf.Clamp(currentSoilAmount + amount, 0, maxSoilAmount);
        UpdateRevealedTiles();
        lastSoilAmount = currentSoilAmount;
    }
    
    public void Reset()
    {
        currentSoilAmount = 0;
        visibleTilemap.ClearAllTiles();
        revealedPositions.Clear();
        lastSoilAmount = -1;
        UpdateRevealedTiles();
    }
}