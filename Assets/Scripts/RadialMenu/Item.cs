using UnityEngine;

public class Item : InteractableObject
{
    public string itemName = "Neznámý předmět";
    public int questID;

    private QuestManager questManager;
    private QuestTablet questTablet;

    void Awake()
    {
        actions.Add("Vylecit se");
        actions.Add("sebrat");

        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("QuestManager instance not found.");
        }

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
        {
            Debug.LogError("QuestTablet instance not found.");
        }
    }

    public override void PerformAction(string action)
    {
        if (action == "sebrat")
        {
            Debug.Log("sebrat");
        }

        if (action == "Vylecit se")
        {
            Debug.Log("Vylecit se");

            if (questManager == null)
            {
                Debug.LogError("QuestManager is not initialized.");
                return;
            }

            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
            if (quest != null)
            {
                if (!quest.isCompleted)
                {
                    Debug.Log("Before update: Quest " + quest.questName + " (ID: " + quest.questID + ") isCompleted: " + quest.isCompleted);

                    questManager.MarkQuestAsCompletedByID(questID);

                    Debug.Log("After update: Quest " + quest.questName + " (ID: " + quest.questID + ") isCompleted: " + quest.isCompleted);
                }
                else
                {
                    Debug.Log("Quest " + quest.questName + " is already completed.");
                }

                if (questTablet != null)
                {
                    questTablet.UpdateQuestList();
                }
                else
                {
                    Debug.LogError("QuestTablet is not initialized.");
                }
            }
            else
            {
                Debug.LogWarning("Quest with ID " + questID + " was not found in active quests.");
            }
        }
    }
}