using UnityEngine;

public class SolarPanelBackButton : MonoBehaviour
{
    public GameObject SolarPanelMinigame;
    
    public void hideSolarPanelMinigame()
    {
        SolarPanelMinigame.SetActive(false);
    }

}
