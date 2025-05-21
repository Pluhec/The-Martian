using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoilDropController : MonoBehaviour
{
    [Header("Reference")]
    public SoilRevealManager soilManager;
    public TextMeshProUGUI soilCounterText;
    public Image loaderImage;
    public SoilQuestManager questManager;

    [Header("Nastavení")]
    public int soilPerItem = 1;
    public KeyCode resetKey = KeyCode.R;

    private void Start()
    {
        RemoveSoil();
    }   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (questManager != null && !questManager.IsQuestActive)
            return;

        if (other.GetComponent<DirtItem>() != null)
        {
            if (soilManager.currentSoilAmount < soilManager.maxSoilAmount)
            {
                soilManager.AddSoil(soilPerItem);
                UpdateUI();
                
                // smazat ze seave systemu aby se potom znovu nespawnul
                if (DroppedItemManager.Instance != null)
                {
                    DroppedItemManager.Instance.RemoveDroppedItem(other.gameObject);
                }
            
                Destroy(other.gameObject);
            }
        }
    }

    public void RemoveSoil()
    {
        soilManager.currentSoilAmount = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (soilCounterText != null)
        {
            soilCounterText.text = $"{soilManager.currentSoilAmount}/{soilManager.maxSoilAmount}";
        }

        if (loaderImage != null)
        {
            float progress = (float)soilManager.currentSoilAmount / soilManager.maxSoilAmount;
            loaderImage.fillAmount = progress;
        }
    }

    public void ResetSystem()
    {
        soilManager.Reset();
        UpdateUI();
    }
}