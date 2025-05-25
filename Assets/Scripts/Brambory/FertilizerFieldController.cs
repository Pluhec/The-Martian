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
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
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
                int potatoQuestIndex = quests.FindIndex(q => q.questID == previousPotatoQuestID);
                if (potatoQuestIndex >= 0 && quests[potatoQuestIndex].isCompleted)
                {
                    shouldShow = true;
                }
            }
        }

        questUIPanel.SetActive(shouldShow);

        CheckQuestCompletion();
    }

    private void Start()
    {
        questManager = QuestManager.Instance;

        LoadFieldState();

        UpdateUI();

        if (!actions.Contains("Fertilize"))
        {
            actions.Add("Fertilize");
        }

        potatoController = FindObjectOfType<PotatoFieldController>();
    }

    private void OnDestroy()
    {
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

    private void LoadFieldState()
    {
        string key = GetPrefKey();

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
        if (currentFertilizerCount >= requiredFertilizerAmount)
        {
            Debug.Log("Pole je již plně pohnojené!");
            return;
        }

        bool isFertilizerQuestActive = IsFertilizerQuestActive();
        if (!isFertilizerQuestActive)
        {
            Debug.Log("Quest na hnojení není aktivní!");
            return;
        }

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

            int slotSize = fertilizerButton.slotSize;
            Inventory.Instance.RemoveItem(fertilizerSlotIndex, slotSize);

            ApplySingleFertilizer();

            Inventory.Instance.AlignItems();

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

        int questIndex = questManager.ActiveQuests.FindIndex(q => q.questID == fertilizerQuestID);
        if (questIndex < 0)
            return false; 

        int potatoQuestIndex = questManager.ActiveQuests.FindIndex(q => q.questID == previousPotatoQuestID);
        if (potatoQuestIndex < 0)
            return false;

        return questManager.ActiveQuests[potatoQuestIndex].isCompleted;
    }

    private void ApplySingleFertilizer()
    {
        if (currentFertilizerCount < requiredFertilizerAmount)
        {
            currentFertilizerCount++;
            UpdateUI();

            if (currentFertilizerCount >= requiredFertilizerAmount)
            {
                CompleteQuest();
            }
        }
        Debug.Log("[ApplyFertilizer] *** Stav inventáře před hledáním hnojiva ***");
        for (int i = 0; i < Inventory.Instance.slots.Length; i++)
        {
            var tf = Inventory.Instance.slots[i].transform;
            Debug.Log($"  Slot {i}: childCount={tf.childCount}");
            if (tf.childCount > 0)
            {
                var go = tf.GetChild(0).gameObject;
                var def = go.GetComponent<ItemDefinition>();
                Debug.Log($"    → Found child '{go.name}', itemID={(def!=null?def.itemID:"<no ItemDefinition>")}");
            }
        }
    }

    private void CompleteQuest()
    {
        if (questManager != null && !questCompleted)
        {
            questManager.MarkQuestAsCompletedByID(fertilizerQuestID);
            questCompleted = true;
            
            if (questUIPanel != null)
                questUIPanel.SetActive(false);

            TimeManager.Instance.ResumeTime();
            
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

