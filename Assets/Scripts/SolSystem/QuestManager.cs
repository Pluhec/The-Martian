using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Aktivní questy")]
    public List<Quest> activeQuests = new List<Quest>();

    // Načte nové questy při začátku solu
    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>();
        foreach (Quest q in quests)
        {
            activeQuests.Add(q);
        }
        Debug.Log("QuestManager: Inicializováno questů: " + activeQuests.Count);
    }

    // Metoda vrací true, pokud jsou všechny questy dokončeny
    public bool AreAllQuestsCompleted()
    {
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isCompleted)
            {
                return false;
            }
        }
        return true;
    }
}