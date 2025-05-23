using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FertilizerFieldController : InteractableObject
{
    [Header("Reference")]
    public TextMeshProUGUI fertilizerCounterText;
    public Image loaderImage;

    [Header("Quest UI")]
    public GameObject questUIPanel;

    [Header("Quest")]
    public int fertilizerQuestID;
    public int previousPotatoQuestID; // ID questu s bramborami

    [Header("Identifikace")]
    [Tooltip("Unikátní ID tohoto pole pro ukládání")]
    public string fertilizerFieldID = "MainField";

    [Header("Nastavení")]
    public int requiredFertilizerAmount = 30;
    public int currentFertilizerCount = 0;
    public string fertilizerItemID = "Fertilizer";

    private QuestManager questManager;
    private bool questCompleted = false;
    private int myQuestIndex = -1;
    private PotatoFieldController potatoController;
    private Collider2D myCollider;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        potatoController = GetComponent<PotatoFieldController>();
        myCollider = GetComponent<Collider2D>();
        
        // Na začátku je tento controller vypnutý
        enabled = false;
    }

    private void OnEnable()
    {
        if (!actions.Contains("Fertilize"))
        {
            actions.Add("Fertilize");
        }
        
        LoadFieldState();
        UpdateUI();
    }

    private void Start()
    {
        questManager = QuestManager.Instance;
    }

    private void Update()
    {
        if (questUIPanel == null || questManager == null || questManager.ActiveQuests == null)
            return;

        bool shouldShow = IsPotatoQuestCompleted() && IsFertilizerQuestActive();
        questUIPanel.SetActive(shouldShow);
    }

    private bool IsPotatoQuestCompleted()
    {
        if (questManager == null || questManager.ActiveQuests == null)
            return false;

        var quest = questManager.ActiveQuests.Find(q => q.questID == previousPotatoQuestID);
        return quest != null && quest.isCompleted;
    }

    private bool IsFertilizerQuestActive()
    {
        if (questManager == null || questManager.ActiveQuests == null)
            return false;

        int index = questManager.ActiveQuests.FindIndex(q => q.questID == fertilizerQuestID);
        if (index < 0) return false;
        
        if (questManager.ActiveQuests[index].isCompleted)
            return false;

        return true;
    }

    public override void PerformAction(string action)
    {
        if (action == "Fertilize")
        {
            ApplyFertilizer();
        }
        else
        {
            base.PerformAction(action);
        }
    }

    private void ApplyFertilizer()
    {
        // Zjistíme počet hnojiva v inventáři
        int fertilizerCount = CountFertilizersInInventory();
        
        if (fertilizerCount <= 0)
        {
            Debug.Log("Nemáš žádné hnojivo k použití!");
            return;
        }

        // Zjistíme, kolik ještě potřebujeme
        int amountNeeded = requiredFertilizerAmount - currentFertilizerCount;
        
        // Určíme kolik hnojiva použijeme (buď všechno co máme nebo jen kolik potřebujeme)
        int amountToUse = Mathf.Min(fertilizerCount, amountNeeded);
        
        // Odebereme hnojivo z inventáře
        RemoveFertilizersFromInventory(amountToUse);
        
        // Přičteme k celkovému počtu
        currentFertilizerCount += amountToUse;
        
        // Aktualizujeme UI
        UpdateUI();
        
        // Uložíme stav
        SaveFieldState();
        
        // Kontrola dokončení questu
        if (currentFertilizerCount >= requiredFertilizerAmount)
        {
            CompleteQuest();
        }
    }

    private int CountFertilizersInInventory()
    {
        int count = 0;
        for (int i = 0; i < Inventory.Instance.slots.Length; i++)
        {
            Transform slotTransform = Inventory.Instance.slots[i].transform;
            if (slotTransform.childCount == 0) continue;

            GameObject slotItem = slotTransform.GetChild(0).gameObject;
            ItemDefinition itemDef = slotItem.GetComponent<ItemDefinition>();
            
            if (itemDef != null && itemDef.itemID == fertilizerItemID)
            {
                ItemButton itemButton = slotItem.GetComponent<ItemButton>();
                if (itemButton != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void RemoveFertilizersFromInventory(int count)
    {
        int removed = 0;
        for (int i = 0; i < Inventory.Instance.slots.Length && removed < count; i++)
        {
            Transform slotTransform = Inventory.Instance.slots[i].transform;
            if (slotTransform.childCount == 0) continue;

            GameObject slotItem = slotTransform.GetChild(0).gameObject;
            ItemButton itemButton = slotItem.GetComponent<ItemButton>();
            if (itemButton == null) continue;

            ItemDefinition itemDef = slotItem.GetComponent<ItemDefinition>();
            if (itemDef != null && itemDef.itemID == fertilizerItemID)
            {
                int slotSize = itemButton.slotSize;
                Inventory.Instance.RemoveItem(i, slotSize);
                removed++;
            }
        }
        
        // Zarovnáme položky v inventáři
        Inventory.Instance.AlignItems();
    }

    private void CompleteQuest()
    {
        if (questManager != null)
        {
            questManager.MarkQuestAsCompletedByID(fertilizerQuestID);
            questCompleted = true;
            
            // Vypneme UI panel
            if (questUIPanel != null)
                questUIPanel.SetActive(false);
            
            // Deaktivujeme všechny komponenty
            enabled = false;
            GetComponent<InteractableObject>().enabled = false;
            if (myCollider != null)
                myCollider.enabled = false;
            
            TimeManager.Instance.ResumeTime();
        }
    }

    private void LoadFieldState()
    {
        string key = GetPrefKey();
        if (PlayerPrefs.HasKey(key))
        {
            currentFertilizerCount = PlayerPrefs.GetInt(key, 0);
            Debug.Log($"Načteno {currentFertilizerCount} hnojiva z PlayerPrefs pro pole {fertilizerFieldID}");
        }
    }

    private void SaveFieldState()
    {
        string key = GetPrefKey();
        PlayerPrefs.SetInt(key, currentFertilizerCount);
        PlayerPrefs.Save();
        Debug.Log($"Uloženo {currentFertilizerCount} hnojiva do PlayerPrefs pro pole {fertilizerFieldID}");
    }

    private string GetPrefKey()
    {
        return $"FertilizerField_{fertilizerFieldID}_Count";
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveFieldState();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void UpdateUI()
    {
        if (fertilizerCounterText != null)
        {
            fertilizerCounterText.text = $"{currentFertilizerCount}/{requiredFertilizerAmount}";
        }

        if (loaderImage != null)
        {
            float progress = (float)currentFertilizerCount / requiredFertilizerAmount;
            loaderImage.fillAmount = progress;
        }
    }

    public void ResetFertilizers()
    {
        currentFertilizerCount = 0;
        UpdateUI();
        SaveFieldState();
    }
}