using UnityEngine;

public class SolarPanel : InteractableObject
{
    public string itemName = "Neznámý předmět";

    public GameObject solarPanel;
    public GameObject playerUI;
    public GameObject questTabletUI;
    public GameObject playerMovement;

    void Awake()
    {
        actions.Add("Clean");
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