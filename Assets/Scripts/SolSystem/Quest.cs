using System;

[Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
    public bool isCompleted;

    // Konstruktor pro snadnou inicializaci questů
    public Quest(string name, string description)
    {
        questName = name;
        questDescription = description;
        isCompleted = false;
    }
}