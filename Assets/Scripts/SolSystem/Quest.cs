using System;

[Serializable]
public class Quest
{
    public int questID;               
    public string questName;          
    public string questDescription;   
    public bool isCompleted;

    // konstruktor questu
    public Quest(int id, string name, string description)
    {
        questID = id;
        questName = name;
        questDescription = description;
        isCompleted = false;
    }
}