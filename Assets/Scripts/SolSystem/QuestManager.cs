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
    }

    // Spočítáme procento dokončených questů
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

    // Oznamujeme splnění questu
    public void MarkQuestAsCompletedByID(int questID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questID == questID && !quest.isCompleted)
            {
                quest.isCompleted = true;
                Debug.Log("Quest " + quest.questName + " completed!");

                // Po dokončení questu zavoláme ResumeTime() v TimeManager pro obnovení času
                TimeManager.Instance.ResumeTime();
                return;
            }
        }
    }

    // Dynamický výpočet cílového času pro questy
    public float GetQuestTargetTime()
    {
        int totalQuests = activeQuests.Count;  // Počet aktivních questů
        float totalDayTime = 14f;  // Celkový čas od 8:00 do 22:00 (14 hodin)

        if (totalQuests == 0)
        {
            return TimeManager.Instance.dayStartTime;  // Pokud nejsou žádné questy, vrátíme 8:00
        }

        // Rozdělíme celkový čas rovnoměrně mezi všechny questy
        float timePerQuest = totalDayTime / totalQuests;  // Čas na jeden quest (v hodinách)
        int activeQuestIndex = activeQuests.FindIndex(q => !q.isCompleted);  // Hledáme první nekompletní quest

        // Pokud není žádný aktivní quest, vrátíme 22:00
        if (activeQuestIndex == -1)
        {
            return TimeManager.Instance.dayEndTime;
        }

        // Počítáme, jaký čas bude přiřazen tomuto konkrétnímu quest
        return TimeManager.Instance.dayStartTime + (activeQuestIndex + 1) * timePerQuest;
    }
}