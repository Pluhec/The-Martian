using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTablet : MonoBehaviour
{
    private QuestManager questManager;
    
    [Header("UI komponenty")]
    public GameObject questEntryPrefab;
    public Transform contentPanel;
    
    private void Awake()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogWarning("QuestTablet.Awake: QuestManager.Instance je null, pokouším se najít instanci pomocí FindObjectOfType.");
            questManager = FindObjectOfType<QuestManager>();
        }
        else
        {
            Debug.Log("QuestTablet.Awake: QuestManager instance byla úspěšně získána.");
        }
    }
    
    private void OnEnable()
    {
        if (questManager == null)
        {
            Debug.LogWarning("QuestTablet.OnEnable: QuestManager je stále null, pokouším se opět najít instanci.");
            questManager = QuestManager.Instance;
            if (questManager == null)
            {
                questManager = FindObjectOfType<QuestManager>();
            }
        }
    }
    
    public void UpdateQuestList()
    {
        Debug.Log("QuestTablet.UpdateQuestList: Aktualizuji seznam questů.");
        
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        Quest questToShow = null;
        int lowestID = int.MaxValue;

        if (questManager == null)
        {
            Debug.LogWarning("QuestTablet: QuestManager reference není nastavena!");
            return;
        }

        foreach (Quest quest in questManager.ActiveQuests)
        {
            if (!quest.isCompleted && quest.questID < lowestID)
            {
                lowestID = quest.questID;
                questToShow = quest;
            }
        }
        
        if (questToShow == null)
        {
            Debug.Log("QuestTablet.UpdateQuestList: Všechny questy jsou splněny.");
            GameObject entry = Instantiate(questEntryPrefab, contentPanel);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts != null && texts.Length >= 1)
            {
                texts[0].text = "Everything's done!";
                if (texts.Length >= 2)
                    texts[1].text = "Get back to Hab and end your sol in the terminal.";
            }
        }
        else
        {
            Debug.Log("QuestTablet.UpdateQuestList: Zobrazím quest " + questToShow.questName);
            GameObject entry = Instantiate(questEntryPrefab, contentPanel);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts != null && texts.Length >= 2)
            {
                texts[0].text = questToShow.questName;
                texts[1].text = questToShow.questDescription;
            }
            else
            {
                Debug.LogWarning("QuestTablet: Quest prefab neobsahuje minimálně 2 TextMeshProUGUI komponenty.");
            }
        }
    }
}