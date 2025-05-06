using UnityEngine;

public class SolarPanel : InteractableObject
{
    public string itemName = "Neznámý předmět";

    public GameObject solarPanel;
    public GameObject playerUI;
    public GameObject questTabletUI;
    public GameObject playerMovement;
    public GameObject SolarPanelMinigame;
    public GameObject inventoryUI;

    public int questID;
    
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
        inventoryUI.SetActive(true);
        
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
            inventoryUI.SetActive(false);

            Movement movementScript = playerMovement.GetComponent<Movement>();
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }
            
            QuestManager questManager = QuestManager.Instance;
            if (questManager != null)
            {
                Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
                if (quest != null && !quest.isCompleted)
                {
                    Debug.Log($"Before clean: Quest {quest.questName} (ID {quest.questID}) isCompleted? {quest.isCompleted}");
                    questManager.MarkQuestAsCompletedByID(questID);
                    Debug.Log($" After clean: Quest {quest.questName} isCompleted? {quest.isCompleted}");
                }
            }
            else
            {
                Debug.LogError("QuestManager instance not found.");
            }
        }
    }
}