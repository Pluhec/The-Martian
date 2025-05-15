using UnityEngine;

public class SoilQuestManager : MonoBehaviour
{
    [Header("Quest Settings")]
    [Tooltip("ID questu, pro který se UI zobrazí až po splnění předchozího")]
    public int questID;

    [Header("UI Elements")]
    [Tooltip("Panel (nebo jiný GameObject), který se má zobrazit/skrýt")]
    public GameObject questUIPanel;

    private QuestManager questManager;
    private int myQuestIndex = -1;

    private void Awake()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("SoilQuestManager: QuestManager.Instance je null!");
    }

    private void Update()
    {
        if (questUIPanel == null || questManager.ActiveQuests == null)
            return;

        var quests = questManager.ActiveQuests;
        
        if (myQuestIndex < 0)
            myQuestIndex = quests.FindIndex(q => q.questID == questID);

        bool shouldShow = false;

        if (myQuestIndex == 0)
        {
            shouldShow = true;
        }
        else if (myQuestIndex > 0 && quests[myQuestIndex - 1].isCompleted)
        {
            shouldShow = true;
        }

        questUIPanel.SetActive(shouldShow); 
    }
}