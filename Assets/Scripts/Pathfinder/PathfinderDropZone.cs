using UnityEngine;

public class PathfinderDropZone : MonoBehaviour
{
    public int targetSol = 91;
    public bool questCompleted = false;
    public string requiredObjectName = "Pathfinder"; // Nová veřejná proměnná pro jméno objektu
    private SpriteRenderer spriteRenderer;
    private QuestManager questManager;
    private QuestTablet questTablet;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisibility();
        
        // Najde QuestManager a QuestTablet pokud nejsou nastavené ručně
        if (questManager == null)
            questManager = FindObjectOfType<QuestManager>();

        if (questTablet == null)
            questTablet = FindObjectOfType<QuestTablet>();
            
        if (questManager == null)
            Debug.LogWarning("QuestManager nebyl nalezen ve scéně!");
    }

    void Update()
    {
        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    {
        int currentSol = GetCurrentSol();
        
        if (currentSol == targetSol)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
    
    private int GetCurrentSol()
    {
        if (GameManager.Instance != null && GameManager.Instance.SolSystem != null)
        {
            return GameManager.Instance.SolSystem.currentSol;
        }
        return 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.name.Contains(requiredObjectName))
        {
            CompleteQuest();
        }
    }
    
    private void CompleteQuest()
    {
        if (questCompleted)
            return;
            
        questCompleted = true;
        Debug.Log($"✅ {requiredObjectName} byl úspěšně doručen na místo!");
        
        // Aktualizace stavu questu, pokud je k dispozici QuestManager
        if (GameManager.Instance != null && GameManager.Instance.QuestManager != null)
        {
            // Tady můžete doplnit logiku pro označení questu jako splněného
            // Například: GameManager.Instance.QuestManager.MarkQuestAsCompletedByID(questID);
        }
        
        // Aktualizace seznamu úkolů, pokud je k dispozici QuestTablet
        if (questTablet != null)
            questTablet.UpdateQuestList();
    }
}
