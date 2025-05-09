using UnityEngine;

public class HabEntryTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public int questID;

    private QuestManager questManager;
    private QuestTablet questTablet;

    private void Awake()
    {
        questManager = QuestManager.Instance;
        questTablet = FindObjectOfType<QuestTablet>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (questManager == null)
            {
                Debug.LogError("QuestManager is not initialized.");
                return;
            }

            Quest quest = questManager.ActiveQuests?.Find(q => q.questID == questID);
            if (quest == null)
            {
                Debug.LogWarning("Quest with ID " + questID + " was not found in active quests.");
                return;
            }

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

            questTablet?.UpdateQuestList();
        } 
    }
}