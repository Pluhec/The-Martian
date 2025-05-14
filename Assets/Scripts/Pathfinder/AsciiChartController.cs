using UnityEngine;
using UnityEngine.UI;

public class AsciiChartSwitcher : MonoBehaviour
{
    [Header("Reference")]
    public Image targetImage;         // Image komponenta, která zobrazuje tabulku

    [Header("Tabulky (Sprite)")]
    public Sprite chart1;
    public Sprite chart2;
    public Sprite chart3;
    public Sprite chart4;

    public void ShowChart1()
    {
        if (targetImage != null && chart1 != null)
            targetImage.sprite = chart1;
    }

    public void ShowChart2()
    {
        if (targetImage != null && chart2 != null)
            targetImage.sprite = chart2;
    }

    public void ShowChart3()
    {
        if (targetImage != null && chart3 != null)
            targetImage.sprite = chart3;
    }

    public void ShowChart4()
    {
        if (targetImage != null && chart4 != null)
            targetImage.sprite = chart4;
    }
}