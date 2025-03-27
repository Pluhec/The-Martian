using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CleanedUI : MonoBehaviour
{
    public Image Bar;
    public TextMeshProUGUI CleanedText;

    public void UpdateBar(float progress)
    {
        Bar.fillAmount = progress;
    }

    public void UpdateText(int cleaned)
    {
        if(cleaned >= 100)
        {
            cleaned = 100;
        }
        
        CleanedText.text = cleaned + "%";
    }
}