using UnityEngine;

public class PathfinderDropZone : MonoBehaviour
{
    [Tooltip("ID questu, který se má dokončit po doručení objektu.")]
    public int questID;
    public int sol;

    private QuestManager questManager;
    private QuestTablet questTablet;
    private bool questCompleted;

    void Awake()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("QuestManager instance not found.");

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
            Debug.LogError("QuestTablet instance not found.");

        // Výchozí stav - neaktivní
        SetChildrenActive(false);

        // Kontrola jestli je správný sol
        int currentSol = SolSystem.Instance.currentSol;
        if (currentSol == sol)
        {
            SetChildrenActive(true);
        }
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
            }

            Destroy(gameObject);
        }
    }
}