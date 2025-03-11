using UnityEngine;

public class HabEntryTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public int questID;

    [Header("Manager References")]
    public QuestManager questManager;
    public QuestTablet questTablet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // vyhledani questu podle nejnizsiho ID v aktivnich questech
            Quest quest = questManager.activeQuests.Find(q => q.questID == questID);
            if (quest != null)
            {
                if (!quest.isCompleted)
                {
                    Debug.Log("Before update: Quest " + quest.questName + " (ID: " + quest.questID + ") isCompleted: " + quest.isCompleted);
                    quest.isCompleted = true;
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
            }
            else
            {
                Debug.LogWarning("Quest with ID " + questID + " was not found in active quests.");
            }
        }
    }
}