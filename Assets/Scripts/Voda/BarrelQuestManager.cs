using UnityEngine;

public class BarrelQuestManager : MonoBehaviour
{
    public int questID;
    public BarrelRevealManager barrelRevealManager;

    private QuestManager questManager;
    private bool questCompleted = false;

    void Awake()
    {
        questManager = QuestManager.Instance;
        if (barrelRevealManager == null)
            barrelRevealManager = FindObjectOfType<BarrelRevealManager>();
    }

    void Update()
    {
        if (questCompleted || barrelRevealManager == null) 
            return;
        
        if (barrelRevealManager.barrel != null && barrelRevealManager.barrel.activeSelf)
        {
            questManager.MarkQuestAsCompletedByID(questID);
            questCompleted = true;
        }
    }
}