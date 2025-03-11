using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SolData
{
    public int solNumber;
    public List<Quest> quests;
    
    public SolData(int sol, List<Quest> questList)
    {
        solNumber = sol;
        quests = questList;
    }
}