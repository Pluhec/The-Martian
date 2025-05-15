using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoilQuestManager : MonoBehaviour
{
    [Header("Quest Settings")]
    public int questID;
    
    [Header("Soil System Components")]
    public SoilRevealManager soilRevealManager;
    public SoilDropController soilDropController;
    public GameObject soilCollectionUI; // Reference na UI prvky pro sběr půdy
    
    [Header("Toggleable Components")]
    public List<GameObject> activateWhenQuestActive = new List<GameObject>(); // Objekty, které se mají aktivovat
    public List<MonoBehaviour> scriptsToEnableWhenActive = new List<MonoBehaviour>(); // Skripty, které se mají povolit
    
    [Header("Completion Settings")]
    [Range(0, 1)]
    public float requiredCompletionPercentage = 1.0f; // Kolik % půdy musí být nasbíráno pro dokončení
    public bool autoCompleteWhenFull = true; // Automaticky dokončit úkol, když je kontejner plný
    
    private QuestManager questManager;
    private QuestTablet questTablet;
    private bool isCurrentlyActive = false;
    private bool isQuestCompleted = false;
    private int currentQuestIndex = -1;
    
    private void Awake()
    {
        questManager = QuestManager.Instance;
        questTablet = FindObjectOfType<QuestTablet>();
        
        if (questManager == null)
            Debug.LogError("❌ QuestManager není inicializován!");
            
        if (soilRevealManager == null || soilDropController == null)
            Debug.LogError("❌ Chybí reference na SoilRevealManager nebo SoilDropController!");
    }
    
    private void Start()
    {
        // Na začátku vše vypnout
        SetComponentsActive(false);
        
        // Pokud je quest již dokončen, nenačítat
        if (questManager != null)
        {
            Quest quest = questManager.ActiveQuests?.Find(q => q.questID == questID);
            if (quest != null)
            {
                if (quest.isCompleted)
                {
                    isQuestCompleted = true;
                    return;
                }
                
                // Najdeme index našeho questu
                currentQuestIndex = questManager.ActiveQuests.FindIndex(q => q.questID == questID);
            }
        }
    }
    
    private void Update()
    {
        CheckQuestStatus();
        
        if (isCurrentlyActive && autoCompleteWhenFull)
        {
            CheckCompletion();
        }
    }
    
    private void CheckQuestStatus()
    {
        if (isQuestCompleted || questManager == null)
            return;
            
        // Zjistit, zda je tento quest aktivní a aktuálně na řadě
        bool isQuestCurrent = questManager.ActiveQuests != null && 
                              currentQuestIndex >= 0 && 
                              currentQuestIndex < questManager.ActiveQuests.Count;
                              
        Quest quest = null;
        if (isQuestCurrent)
        {
            quest = questManager.ActiveQuests?.Find(q => q.questID == questID && !q.isCompleted);
            isQuestCurrent = quest != null;
        }
        
        // Pokud došlo ke změně stavu, aktualizovat komponenty
        if (isQuestCurrent != isCurrentlyActive)
        {
            isCurrentlyActive = isQuestCurrent;
            SetComponentsActive(isCurrentlyActive);
            
            if (isCurrentlyActive)
            {
                Debug.Log($"🌱 Quest '{quest?.questName}' aktivován - spouštím systém sběru půdy (ID: {questID})");
                // Resetovat půdu při aktivaci questu
                if (soilDropController != null)
                {
                    soilDropController.ResetSystem();
                }
            }
            else
            {
                Debug.Log("🌱 Quest sběru půdy již není aktivní - vypínám systém");
            }
        }
    }
    
    private void SetComponentsActive(bool active)
    {
        // Aktivace/deaktivace UI
        if (soilCollectionUI != null)
            soilCollectionUI.SetActive(active);
            
        // Aktivace/deaktivace všech GameObject
        foreach (GameObject obj in activateWhenQuestActive)
        {
            if (obj != null)
                obj.SetActive(active);
        }
        
        // Povolení/zakázání všech skriptů
        foreach (MonoBehaviour script in scriptsToEnableWhenActive)
        {
            if (script != null)
                script.enabled = active;
        }
    }
    
    private void CheckCompletion()
    {
        if (soilRevealManager == null)
            return;
            
        float currentPercentage = (float)soilRevealManager.currentSoilAmount / soilRevealManager.maxSoilAmount;
        
        // Quest je dokončen, když množství půdy dosáhne určeného procenta
        if (currentPercentage >= requiredCompletionPercentage && !isQuestCompleted)
        {
            CompleteQuest();
        }
    }
    
    private void CompleteQuest()
    {
        if (questManager == null || isQuestCompleted)
            return;
            
        Quest quest = questManager.ActiveQuests?.Find(q => q.questID == questID);
        if (quest != null && !quest.isCompleted)
        {
            Debug.Log($"✅ Dokončuji quest '{quest.questName}' (ID: {questID}) - kontejner na půdu je plný");
            questManager.MarkQuestAsCompletedByID(questID);
            isQuestCompleted = true;
            
            // Aktualizovat tablet s úkoly
            if (questTablet != null)
                questTablet.UpdateQuestList();
                
            // Vypnout komponenty
            SetComponentsActive(false);
        }
    }
    
}