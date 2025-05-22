// PotatoProgressUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotatoProgressUI : MonoBehaviour
{
    [Header("Brambory")]
    public TextMeshProUGUI plantedText;
    public Image plantedFill;

    [Header("Hnojení")]
    public TextMeshProUGUI fertilizedText;
    public Image fertilizedFill;

    private int total;

    /// <summary>
    /// Nastaví celkový počet (např. 30).
    /// </summary>
    public void Initialize(int totalCount)
    {
        total = totalCount;
        UpdatePlanted(0);
        UpdateFertilized(0);
    }

    public void UpdatePlanted(int planted)
    {
        plantedText.text = $"{planted}/{total}";
        if (plantedFill) plantedFill.fillAmount = (float)planted / total;
    }

    public void UpdateFertilized(int fertilized)
    {
        fertilizedText.text = $"{fertilized}/{total}";
        if (fertilizedFill) fertilizedFill.fillAmount = (float)fertilized / total;
    }
}