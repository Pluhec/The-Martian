using UnityEngine;

public class PathfinderDropZone : MonoBehaviour
{
    [Tooltip("ID questu, který se má dokončit po doručení objektu.")]
    public int questID;

    [Tooltip("Den (Sol), ve kterém je zóna aktivní.")]
    public int targetSol = 91;

    [Tooltip("Část jména objektu, který se má doručit.")]
    public string requiredObjectName = "Pathfinder";

    private bool questCompleted = false;
    private SpriteRenderer spriteRenderer;
    private QuestManager questManager;
    private QuestTablet questTablet;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
        if (QuestManager.Instance != null)
            questManager = QuestManager.Instance;
    }

    void Start()
    {
        if (questManager == null)
            questManager = FindObjectOfType<QuestManager>();

        questTablet = FindObjectOfType<QuestTablet>();

        if (questManager == null)
            Debug.LogError("❌ QuestManager nebyl nalezen ve scéně!");
        if (questTablet == null)
            Debug.LogWarning("⚠️ QuestTablet nebyl nalezen – seznam úkolů se neaktualizuje vizuálně.");

        UpdateVisibility();
    }

    void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        spriteRenderer.enabled = (GetCurrentSol() == targetSol);
    }

    private int GetCurrentSol()
    {
        return GameManager.Instance?.SolSystem?.currentSol ?? 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (questCompleted)
            return;

        if (other.CompareTag("Interactable") && other.name.Contains(requiredObjectName))
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        if (questManager == null)
        {
            Debug.LogError("❌ QuestManager není inicializován!");
            return;
        }
        
        Quest quest = questManager.ActiveQuests?.Find(q => q.questID == questID);
        if (quest == null)
        {
            Debug.LogWarning("⚠️ Quest s ID " + questID + " nebyl nalezen mezi aktivními questy.");
            return;
        }

        if (!quest.isCompleted)
        {
            Debug.Log("ℹ️ Před dokončením: Quest " + quest.questName +
                      " (ID: " + quest.questID + ") isCompleted = " + quest.isCompleted);
            
            questManager.MarkQuestAsCompletedByID(questID);
            questCompleted = true;

            Debug.Log("✅ Po dokončení: Quest " + quest.questName +
                      " (ID: " + quest.questID + ") isCompleted = " + quest.isCompleted);
            
            questTablet?.UpdateQuestList();
        }
        else
        {
            Debug.Log("ℹ️ Quest " + quest.questName + " je již dokončen.");
        }
    }
}