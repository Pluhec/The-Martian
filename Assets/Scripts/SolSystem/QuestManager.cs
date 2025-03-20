using UnityEngine;
using System.Collections.Generic;

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
        ResetQuestTimers();
    }
    
    public void ResetQuestTimers()
    {
        foreach (var quest in activeQuests)
        {
            quest.isCompleted = false;
        }
    }

    public float GetQuestCompletionPercentage()
    {
        int completedCount = 0;
        foreach (Quest quest in activeQuests)
        {
            if (quest.isCompleted)
                completedCount++;
        }
        return (float)completedCount / activeQuests.Count;
    }

    public bool AreAllQuestsCompleted()
    {
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isCompleted)
                return false;
        }
        return true;
    }

    public void MarkQuestAsCompletedByID(int questID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questID == questID && !quest.isCompleted)
            {
                quest.isCompleted = true;
                Debug.Log("Quest " + quest.questName + " completed!");
                
                TimeManager.Instance.ResumeTime();
                return;
            }
        }
    }

    public float GetQuestTargetTime()
    {
        int totalQuests = activeQuests.Count;
        float totalDayTime = 14f;

        if (totalQuests == 0)
        {
            return TimeManager.Instance.dayStartTime;
        }
        
        float timePerQuest = totalDayTime / totalQuests;
        int activeQuestIndex = activeQuests.FindIndex(q => !q.isCompleted); 

        if (activeQuestIndex == -1)
        {
            return TimeManager.Instance.dayEndTime;
        }

        return TimeManager.Instance.dayStartTime + (activeQuestIndex + 1) * timePerQuest;
    }
}