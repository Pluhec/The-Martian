using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CleanedUI : MonoBehaviour
{
    public Image Bar;
    public TextMeshProUGUI CleanedText;

    private AudioManager audioManager;
    private bool hasPlayedFullyCleanedSound = false;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void UpdateBar(float progress)
    {
        Bar.fillAmount = progress;
    }

    public void UpdateText(int cleaned)
    {
        if (cleaned >= 100) cleaned = 100;

        if (cleaned == 100 && !hasPlayedFullyCleanedSound)
        {
            hasPlayedFullyCleanedSound = true;
            PlayFullyCleanedSound();
        }

        CleanedText.text = cleaned + "%";
    }

    private void PlayFullyCleanedSound()
    {
        audioManager.PlayFullyCleanedSolarPanel(audioManager.FullyCleanedSound);
    }
    
    public void ResetSoundFlag()
    {
        hasPlayedFullyCleanedSound = false;
    }
}