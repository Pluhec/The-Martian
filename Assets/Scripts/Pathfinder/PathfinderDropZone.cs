using UnityEngine;

public class PathfinderDropZone : MonoBehaviour
{
    [Tooltip("ID questu, který se má dokončit po doručení objektu.")]
    public int questID;

    private QuestManager questManager;
    private QuestTablet questTablet;
    private bool questCompleted;
    private bool isActive;

    void Awake()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("QuestManager instance not found.");

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
            Debug.LogError("QuestTablet instance not found.");

        SetChildrenActive(false);
    }
    
    void OnBecameVisible()
    {
        Debug.Log($"{name} je nyní viditelný kamerou.");
    }

    void Update()
    {
        Quest currentQuest = questManager.ActiveQuests[questManager.currentQuestIndex];
        bool shouldBeActive = currentQuest.questID == questID;
        
        SetChildrenActive(shouldBeActive);
    }

    private void SetChildrenActive(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (questCompleted) return;

        Debug.Log($"Collision detected with: {other.gameObject.name}, Tag: {other.tag}");

        if (other.GetComponent<PathfinderComputer>() != null)
        {
            Quest quest = questManager.ActiveQuests[questManager.currentQuestIndex];
            if (quest.questID == questID && !quest.isCompleted)
            {
                Debug.Log($"Before completing: Quest {quest.questName} (ID {quest.questID}) isCompleted? {quest.isCompleted}");
                questManager.MarkQuestAsCompletedByID(questID);
                Debug.Log($"After completing: Quest {quest.questName} isCompleted? {quest.isCompleted}");

                if (questTablet != null)
                    questTablet.UpdateQuestList();

                questCompleted = true;
                SetChildrenActive(false);
                isActive = false;
            }

            Destroy(gameObject);
        }
    }
}