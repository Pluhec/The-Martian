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
        // Registrujeme metodu pro ukládání při změně scény
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Kontrola a aktualizace stavu UI podle questu
        if (questUIPanel == null || questManager == null || questManager.ActiveQuests == null)
            return;

        var quests = questManager.ActiveQuests;

        if (myQuestIndex < 0)
            myQuestIndex = quests.FindIndex(q => q.questID == fertilizerQuestID);

        bool shouldShow = false;

        if (myQuestIndex >= 0 && myQuestIndex < quests.Count)
        {
            if (!quests[myQuestIndex].isCompleted)
            {
                // Quest pro fertilizer by měl být aktivní pouze pokud předchozí (brambory) je dokončen
                int potatoQuestIndex = quests.FindIndex(q => q.questID == previousPotatoQuestID);
                if (potatoQuestIndex >= 0 && quests[potatoQuestIndex].isCompleted)
                {
                    shouldShow = true;
                }
            }
        }

        questUIPanel.SetActive(shouldShow);

        // Kontroluje dokončení questu, když je pole plně pohnojené
        CheckQuestCompletion();
    }

    private void Start()
    {
        // Získání reference na QuestManager
        questManager = QuestManager.Instance;

        // Načtení stavu pole
        LoadFieldState();

        // Aktualizace UI
        UpdateUI();

        // Přidání akce Fertilize, pokud neexistuje
        if (!actions.Contains("Fertilize"))
        {
            actions.Add("Fertilize");
        }

        // Vyhledání reference na PotatoFieldController
        potatoController = FindObjectOfType<PotatoFieldController>();
    }

    private void OnDestroy()
    {
        // Odregistrujeme event při zničení objektu
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void CheckQuestCompletion()
    {
        if (questCompleted || questManager == null)
            return;

        if (currentFertilizerCount >= requiredFertilizerAmount)
        {
            CompleteQuest();
        }
    }

    // Načtení stavu pole
    private void LoadFieldState()
    {
        string key = GetPrefKey();

        // Načtení uloženého počtu hnojiva
        if (PlayerPrefs.HasKey(key))
        {
            currentFertilizerCount = PlayerPrefs.GetInt(key, 0);
            Debug.Log($"Načteno {currentFertilizerCount} hnojiva z PlayerPrefs pro pole {fertilizerFieldID}");
        }
        else
        {
            ResetFertilizer();
        }
    }

    // Uložení stavu pole
    private void SaveFieldState()
    {
        string key = GetPrefKey();
        PlayerPrefs.SetInt(key, currentFertilizerCount);
        PlayerPrefs.Save();
        Debug.Log($"Uloženo {currentFertilizerCount} hnojiva do PlayerPrefs pro pole {fertilizerFieldID}");
    }

    // Vytvoření klíče pro PlayerPrefs
    private string GetPrefKey()
    {
        return $"FertilizerField_{fertilizerFieldID}_Count";
    }

    // Volá se při opuštění scény
    private void OnSceneUnloaded(Scene scene)
    {
        SaveFieldState();
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
        // Kontrola maximálního množství hnojiva
        if (currentFertilizerCount >= requiredFertilizerAmount)
        {
            Debug.Log("Pole je již plně pohnojené!");
            return;
        }

        // Kontrola správného questu pro hnojení
        bool isFertilizerQuestActive = IsFertilizerQuestActive();
        if (!isFertilizerQuestActive)
        {
            Debug.Log("Quest na hnojení není aktivní!");
            return;
        }

        // Hledání hnojiva v inventáři
        bool foundFertilizer = false;
        ItemButton fertilizerButton = null;
        int fertilizerSlotIndex = -1;

        for (int i = 0; i < Inventory.Instance.slots.Length; i++)
        {
            Transform slotTransform = Inventory.Instance.slots[i].transform;
            if (slotTransform.childCount == 0) continue;

            GameObject slotItem = slotTransform.GetChild(0).gameObject;
            ItemButton itemButton = slotItem.GetComponent<ItemButton>();
            if (itemButton == null) continue;

            ItemDefinition itemDef = slotItem.GetComponent<ItemDefinition>();
            if (itemDef != null && itemDef.itemID == fertilizerItemID)
            {
                foundFertilizer = true;
                fertilizerButton = itemButton;
                fertilizerSlotIndex = i;
                break;
            }
        }

        if (foundFertilizer && fertilizerButton != null)
        {
            Debug.Log("Hnojivo nalezeno v inventáři, odstraňuji...");

            // Použití vestavěné metody z Inventory pro odstranění položky
            int slotSize = fertilizerButton.slotSize;
            Inventory.Instance.RemoveItem(fertilizerSlotIndex, slotSize);

            // Přidání hnojiva na pole
            ApplySingleFertilizer();

            // Zarovnání inventáře po odstranění
            Inventory.Instance.AlignItems();

            // Uložení stavu po každém použití hnojiva
            SaveFieldState();
        }
        else
        {
            Debug.Log("Nemáš žádné hnojivo k použití!");
        }
    }

    private bool IsFertilizerQuestActive()
    {
        if (questManager == null)
            return false;

        if (questManager.ActiveQuests == null)
            return false;

        // Hledání questu s daným ID
        int questIndex = questManager.ActiveQuests.FindIndex(q => q.questID == fertilizerQuestID);
        if (questIndex < 0)
            return false; // Quest neexistuje

        // Kontrola, zda je předchozí quest (brambory) dokončen
        int potatoQuestIndex = questManager.ActiveQuests.FindIndex(q => q.questID == previousPotatoQuestID);
        if (potatoQuestIndex < 0)
            return false; // Předchozí quest neexistuje

        return questManager.ActiveQuests[potatoQuestIndex].isCompleted;
    }

    private void ApplySingleFertilizer()
    {
        if (currentFertilizerCount < requiredFertilizerAmount)
        {
            currentFertilizerCount++;
            UpdateUI();

            // Kontrola, jestli je pole plně pohnojeno
            if (currentFertilizerCount >= requiredFertilizerAmount)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        if (questManager != null && !questCompleted)
        {
            questManager.MarkQuestAsCompletedByID(fertilizerQuestID);
            questCompleted = true;
            
            // Vypneme UI panel po dokončení questu
            if (questUIPanel != null)
                questUIPanel.SetActive(false);

            // Vypneme celý GameObject
            TimeManager.Instance.ResumeTime();
            
            // Deaktivujeme celý GameObject
            gameObject.SetActive(false);
        }
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

    public void ResetFertilizer()
    {
        currentFertilizerCount = 0;
        UpdateUI();
        SaveFieldState();
    }
}