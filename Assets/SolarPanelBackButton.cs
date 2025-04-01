using UnityEngine;

public class SolarPanelBackButton : MonoBehaviour
{
    public GameObject SolarPanelMinigame;
    private AudioManager audioManager;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    public void hideSolarPanelMinigame()
    {
        audioManager.StopCleaningSolarPanel();
        SolarPanelMinigame.SetActive(false);
    }

}
