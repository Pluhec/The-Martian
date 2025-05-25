using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PotatoFieldController : InteractableObject
{
    [Header("Reference")]
    public GameObject[] potatoPrefabs;
    public TextMeshProUGUI potatoCounterText;
    public Image loaderImage;

    [Header("Quest UI")]
    public GameObject questUIPanel;

    [Header("Quest")]
    public int potatoQuestID;

    [Header("Identifikace")]
    [Tooltip("Unikátní ID tohoto bramborového pole pro ukládání")]
    public string potatoFieldID = "MainField";

    [Header("Návaznost")]
    [Tooltip("GameObject, který se aktivuje po dokončení bramborového questu")]
    public GameObject fertilizerGameObject;

    [Header("Nastavení")]
    public int maxPotatoCount = 30;
    public int currentPotatoCount = 0;
    public string potatoItemID = "Potato";

    private QuestManager questManager;
    private bool questCompleted = false;
    private int myQuestIndex = -1;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void Update()
    {
        if (questUIPanel == null || questManager == null || questManager.ActiveQuests == null)
            return;

        var quests = questManager.ActiveQuests;

        if (myQuestIndex < 0)
            myQuestIndex = quests.FindIndex(q => q.questID == potatoQuestID);

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

        questUIPanel.SetActive(shouldShow);
        CheckQuestCompletion();
    }

    private void CheckQuestCompletion()
    {
        if (questCompleted || questManager == null)
            return;

        if (currentPotatoCount >= maxPotatoCount)
        {
            CompleteQuest();
            questCompleted = true;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Start()
    {
        questManager = QuestManager.Instance;
        LoadFieldState();
        CheckCurrentSol();
        UpdateUI();

        if (!actions.Contains("Plant"))
        {
            actions.Add("Plant");
        }
    }

    private void CheckCurrentSol()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            bool isSol30 = (SolSystem.Instance != null && SolSystem.Instance.currentSol == 30);
            myCollider.enabled = isSol30;
            Debug.Log($"[PotatoFieldController] Collider {(isSol30 ? "zapnut" : "vypnut")} - Sol {(isSol30 ? "30" : SolSystem.Instance?.currentSol.ToString() ?? "neznámý")}");
        }
        else
        {
            Debug.LogWarning("[PotatoFieldController] Nebyl nalezen žádný Collider2D komponenta!");
        }
    }

    private void LoadFieldState()
    {
        string key = GetPrefKey();

        if (PlayerPrefs.HasKey(key))
        {
            currentPotatoCount = PlayerPrefs.GetInt(key, 0);
            Debug.Log($"Načteno {currentPotatoCount} brambor z PlayerPrefs pro pole {potatoFieldID}");

            for (int i = 0; i < currentPotatoCount && i < potatoPrefabs.Length; i++)
            {
                potatoPrefabs[i].SetActive(true);
            }
        }
        else
        {
            ResetPotatoes();
        }
    }

    private void SaveFieldState()
    {
        string key = GetPrefKey();
        PlayerPrefs.SetInt(key, currentPotatoCount);
        PlayerPrefs.Save();
        Debug.Log($"Uloženo {currentPotatoCount} brambor do PlayerPrefs pro pole {potatoFieldID}");
    }

    private string GetPrefKey()
    {
        return $"PotatoField_{potatoFieldID}_Count";
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveFieldState();
    }

    public override void PerformAction(string action)
    {
        if (action == "Plant")
        {
            PlantPotato();
        }
        else
        {
            base.PerformAction(action);
        }
    }

    private void PlantPotato()
    {
        if (currentPotatoCount >= maxPotatoCount)
        {
            Debug.Log("Bramborové pole je již plné!");
            return;
        }

        bool isPotatoQuestActive = IsPotatoQuestActive();
        if (!isPotatoQuestActive)
        {
            Debug.Log("Quest na brambory není aktivní!");
            return;
        }

        bool foundPotato = false;
        ItemButton potatoButton = null;
        int potatoSlotIndex = -1;

        for (int i = 0; i < Inventory.Instance.slots.Length; i++)
        {
            Transform slotTransform = Inventory.Instance.slots[i].transform;
            if (slotTransform.childCount == 0) continue;

            GameObject slotItem = slotTransform.GetChild(0).gameObject;
            ItemButton itemButton = slotItem.GetComponent<ItemButton>();
            if (itemButton == null) continue;

            ItemDefinition itemDef = slotItem.GetComponent<ItemDefinition>();
            if (itemDef != null && itemDef.itemID == potatoItemID)
            {
                foundPotato = true;
                potatoButton = itemButton;
                potatoSlotIndex = i;
                break;
            }
        }

        if (foundPotato && potatoButton != null)
        {
            Debug.Log($"[PlantPotato] Brambora nalezena v slotu {potatoSlotIndex}, slotSize = {potatoButton.slotSize}");
            Inventory.Instance.RemoveItem(potatoSlotIndex, potatoButton.slotSize);
            Debug.Log($"[PlantPotato] Voláno RemoveItem({potatoSlotIndex}, {potatoButton.slotSize})");
            PlantSinglePotato();
            Inventory.Instance.AlignItems();
            Debug.Log("[PlantPotato] Inventář zarovnán po odstranění");
            SaveFieldState();
        }
        else
        {
            Debug.Log("Nemáš žádnou bramboru k zasazení!");
        }
    }

    private bool IsPotatoQuestActive()
    {
        if (questManager == null)
            return true;

        if (questManager.ActiveQuests == null)
            return false;

        int questIndex = questManager.ActiveQuests.FindIndex(q => q.questID == potatoQuestID);
        if (questIndex < 0)
            return false;

        if (questIndex == 0)
            return true;
        else
            return questManager.ActiveQuests[questIndex - 1].isCompleted;
    }

    private void PlantSinglePotato()
    {
        if (currentPotatoCount < maxPotatoCount && currentPotatoCount < potatoPrefabs.Length)
        {
            potatoPrefabs[currentPotatoCount].SetActive(true);
            currentPotatoCount++;
            UpdateUI();

            if (currentPotatoCount >= maxPotatoCount)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        if (questManager != null)
        {
            questManager.MarkQuestAsCompletedByID(potatoQuestID);
            questCompleted = true;

            if (questUIPanel != null)
                questUIPanel.SetActive(false);

            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
                myCollider.enabled = false;

            actions.Clear();

            if (fertilizerGameObject != null)
                fertilizerGameObject.SetActive(true);

            this.enabled = false;

            TimeManager.Instance.ResumeTime();
        }
    }

    private void UpdateUI()
    {
        if (potatoCounterText != null)
        {
            potatoCounterText.text = $"{currentPotatoCount}/{maxPotatoCount}";
        }

        if (loaderImage != null)
        {
            float progress = (float)currentPotatoCount / maxPotatoCount;
            loaderImage.fillAmount = progress;
        }
    }

    public void ResetPotatoes()
    {
        currentPotatoCount = 0;
        foreach (GameObject potato in potatoPrefabs)
        {
            potato.SetActive(false);
        }
        UpdateUI();
        SaveFieldState();
    }
}