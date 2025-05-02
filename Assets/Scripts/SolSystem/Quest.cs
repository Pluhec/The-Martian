using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Quest
{
    public int questID;
    public string questName;
    public string questDescription;
    public bool isCompleted;
    [NonSerialized] public int currentTargetIndex = 0;
    public Quest(int id, string name, string description)
    {
        questID = id;
        questName = name;
        questDescription = description;
        isCompleted = false;
        currentTargetIndex = 0;
    }
}