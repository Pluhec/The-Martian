using UnityEngine;

public class SoilQuestManager : MonoBehaviour
{
    public int questID;

    public GameObject questUIPanel;

    public SoilRevealManager soilRevealManager;

    private QuestManager questManager;
    private QuestTablet questTablet;
    private int myQuestIndex = -1;
    private bool questCompleted = false;
    
    public bool IsQuestActive { get; private set; } = false;

    private void Awake()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("questManager neni");

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
            Debug.LogWarning("questTablet neni");

        if (soilRevealManager == null)
            soilRevealManager = FindObjectOfType<SoilRevealManager>();
    }

    private void Update()
    {
        if (questUIPanel == null || questManager.ActiveQuests == null)
            return;

        var quests = questManager.ActiveQuests;

        if (myQuestIndex < 0)
            myQuestIndex = quests.FindIndex(q => q.questID == questID);

        bool shouldShow = false;

        if (myQuestIndex >= 0 && myQuestIndex < quests.Count)
        {
            if (!quests[myQuestIndex].isCompleted)
            {
                if (myQuestIndex == 0)
                {
                    shouldShow = true;
                }
                else if (myQuestIndex > 0 && quests[myQuestIndex - 1].isCompleted)
                {
                    shouldShow = true;
                }
            }
        }
        
        IsQuestActive = shouldShow;
        questUIPanel.SetActive(shouldShow);

        CheckQuestCompletion();
    }

    private void CheckQuestCompletion()
    {
        if (questCompleted || soilRevealManager == null || questManager == null)
            return;

        if (soilRevealManager.currentSoilAmount >= soilRevealManager.maxSoilAmount)
        {
            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
            if (quest != null && !quest.isCompleted)
            {
                questManager.MarkQuestAsCompletedByID(questID);

                if (questTablet != null)
                    questTablet.UpdateQuestList();

                TimeManager.Instance.ResumeTime();

                questCompleted = true;
            }
        }
    }
}