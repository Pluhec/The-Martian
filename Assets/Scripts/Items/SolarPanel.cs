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
        if (inventoryUI == null)
            FindInventory();
    }
    
    private void FindInventory()
    {
        inventoryUI = GameObject.Find("InventoryManager")
            ?? FindObjectOfType<Inventory>()?.gameObject;
    }

    public override void PerformAction(string action)
    {
        if (action != "Clean") 
            return;
        
        if (inventoryUI == null)
            FindInventory();
        
        var clean = SolarPanelMinigame.GetComponentInChildren<Clean>();
        
        var qm = QuestManager.Instance;
        var quest = qm?
            .ActiveQuests
            .Find(q => q.questID == questID);
        
        if (quest != null && !quest.isCompleted)
            clean.ResetToDirty();
        else
            clean.ClearDirtPermanently();
        
        solarPanel.SetActive(true);
        playerUI.SetActive(false);
        questTabletUI.SetActive(false);
        inventoryUI.SetActive(false);
        var move = playerMovement.GetComponent<Movement>();
        if (move != null) move.enabled = false;
        
        if (qm != null && quest != null && !quest.isCompleted)
        {
            Debug.Log($"Before clean: Quest {quest.questName} isCompleted? {quest.isCompleted}");
            qm.MarkQuestAsCompletedByID(questID);
            Debug.Log($" After clean: Quest {quest.questName} isCompleted? {quest.isCompleted}");
        }
        else if (qm == null)
        {
            Debug.LogError("QuestManager instance not found.");
        }
    }

    public void hideSolarPanelMinigame()
    {
        audioManager.StopCleaningSolarPanel();
        SolarPanelMinigame.SetActive(false);
        playerUI.SetActive(true);
        questTabletUI.SetActive(true);
        inventoryUI.SetActive(true);
        var move = playerMovement.GetComponent<Movement>();
        if (move != null) move.enabled = true;
    }
}