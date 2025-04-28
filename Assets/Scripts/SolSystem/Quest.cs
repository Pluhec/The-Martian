using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Quest
{
    public int questID;
    public string questName;
    public string questDescription;
    public bool isCompleted;
    
    public List<Transform> targets = new List<Transform>();
    [NonSerialized] public int currentTargetIndex = 0;

    public Quest(int id, string name, string description, List<Transform> targetList)
    {
        questID = id;
        questName = name;
        questDescription = description;
        isCompleted = false;
        targets = targetList;
        currentTargetIndex = 0;
    }
    
    public Transform GetCurrentTarget()
    {
        if (currentTargetIndex < targets.Count)
            return targets[currentTargetIndex];
        return null;
    }
}