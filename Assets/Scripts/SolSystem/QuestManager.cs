using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    
    private List<Quest> activeQuests = new List<Quest>();
    
    public List<Quest> ActiveQuests { get { return activeQuests; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>(quests);
    }
    
// jestli jsou vsechny questy splene:
    public bool AreAllQuestsCompleted()
    {
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isCompleted)
                return false;
        }
        return true;
    }

    // oznameni splneni questu
    public void MarkQuestAsCompletedByID(int questID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questID == questID && !quest.isCompleted)
            {
                quest.isCompleted = true;
                Debug.Log("QuestManager: Quest s ID " + questID + " byl označen jako splněný.");
                return;
            }
        }
        Debug.LogWarning("QuestManager: Nesplněný quest s ID " + questID + " nebyl nalezen.");
    }
}