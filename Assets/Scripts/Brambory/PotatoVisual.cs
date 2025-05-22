using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PotatoVisual : MonoBehaviour
{
    public Sprite[] growthStages;       // 0 = zasazená, …, n = zralá
    private SpriteRenderer sr;
    private int growthStage = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    /// <summary>
    /// Posune růst o jeden stupeň, pokud ještě nejsem na max.
    /// </summary>
    public void AdvanceGrowth()
    {
        if (growthStage < growthStages.Length - 1)
        {
            growthStage++;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        if (growthStages != null && growthStages.Length > 0)
            sr.sprite = growthStages[growthStage];
    }

    /// <summary>
    /// Obnoví fázi růstu (volitelné).
    /// </summary>
    public void ResetGrowth()
    {
        growthStage = 0;
        UpdateVisual();
    }
}