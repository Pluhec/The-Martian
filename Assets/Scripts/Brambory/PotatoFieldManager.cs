// PotatoFieldManager.cs
using UnityEngine;
using System.Collections.Generic;

public class PotatoFieldManager : MonoBehaviour
{
    public static PotatoFieldManager Instance { get; private set; }
    
    public bool CanPlant => plantedCount < totalPotatoes;
    public bool CanFertilize => fertilizedCount < plantedCount;

    [Header("Brambory ve scéně (nezapomeň vložit 30)")]
    public List<PotatoVisual> potatoes;

    [Header("UI")]
    public PotatoProgressUI ui;

    [Header("Nastavení")]
    public int totalPotatoes = 30;

    private int plantedCount = 0;
    private int fertilizedCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // všechny brambory na začátku vypnuté
        foreach (var p in potatoes) p.gameObject.SetActive(false);
        plantedCount = fertilizedCount = 0;
        if (ui != null) ui.Initialize(totalPotatoes);
    }

    /// <summary>
    /// Zavolá se ze PotatoSoilSpot při zásazení.
    /// </summary>
    public bool PlantOne()
    {
        if (plantedCount >= totalPotatoes) return false;
        var p = potatoes[plantedCount];
        p.gameObject.SetActive(true);
        plantedCount++;
        ui?.UpdatePlanted(plantedCount);
        return true;
    }

    /// <summary>
    /// Zavolá se ze PotatoSoilSpot při hnojení.
    /// </summary>
    public bool FertilizeOne()
    {
        if (fertilizedCount >= plantedCount) return false;
        fertilizedCount++;
        ui?.UpdateFertilized(fertilizedCount);
        return true;
    }

    /// <summary>
    /// Zavolá SolSystem v novém solu.
    /// </summary>
    public void UpdatePotatoesGrowth()
    {
        for (int i = 0; i < fertilizedCount; i++)
            potatoes[i].AdvanceGrowth();
    }
}