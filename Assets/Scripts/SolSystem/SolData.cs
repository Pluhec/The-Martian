using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SolData
{
    public int solNumber;
    public List<Quest> quests;

    // Konstruktor, který nastaví číslo solu a seznam questů
    public SolData(int sol, List<Quest> questList)
    {
        solNumber = sol;
        quests = questList;
    }
}