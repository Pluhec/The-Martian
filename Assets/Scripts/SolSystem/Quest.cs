using System;

[Serializable]
public class Quest
{
    public string questName;
    public bool isCompleted;

    // Konstruktor pro snadnou inicializaci questů
    public Quest(string name)
    {
        questName = name;
        isCompleted = false;
    }
}