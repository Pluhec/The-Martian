using UnityEngine;

public class SolarPanel : InteractableObject
{
    public string itemName = "Neznámý předmět";

    public GameObject solarPanel;
    public GameObject playerUI;
    public GameObject questTabletUI;
    public GameObject playerMovement;
    public GameObject SolarPanelMinigame;
    
    private AudioManager audioManager;
    
    private void Awake()
    {
        actions.Add("Clean");
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    public void hideSolarPanelMinigame()
    {
        audioManager.StopCleaningSolarPanel();
        SolarPanelMinigame.SetActive(false);
        
        playerUI.SetActive(true);
        questTabletUI.SetActive(true);
        
        Movement movementScript = playerMovement.GetComponent<Movement>();
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }

    public override void PerformAction(string action)
    {
        if (action == "Clean")
        {
            solarPanel.SetActive(true);
            playerUI.SetActive(false);
            questTabletUI.SetActive(false);

            Movement movementScript = playerMovement.GetComponent<Movement>();
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }
        }
    }
}