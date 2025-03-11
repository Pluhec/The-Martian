using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Aktivní questy")]
    public List<Quest> activeQuests = new List<Quest>();

    // nacteni questu na zacatku solu
    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>();
        foreach (Quest q in quests)
        {
            activeQuests.Add(q);
        }
    }

    // jestli jsou vsechny questy splene:
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