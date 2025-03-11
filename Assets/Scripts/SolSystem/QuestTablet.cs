using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTablet : MonoBehaviour
{
    [Header("Reference na QuestManager")]
    public QuestManager questManager;
    
    [Header("UI komponenty")]
    public GameObject questEntryPrefab;
    public Transform contentPanel;
    
    public void UpdateQuestList()
    {
        // promazani celeho seznamu questu
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // nejblizsi quest ktery neni hotovy
        Quest questToShow = null;
        int lowestID = int.MaxValue;

        foreach (Quest quest in questManager.activeQuests)
        {
            if (!quest.isCompleted && quest.questID < lowestID)
            {
                lowestID = quest.questID;
                questToShow = quest;
            }
        }
        
        if (questToShow == null)
        {
            // nejsou questy - tato zprava
            GameObject entry = Instantiate(questEntryPrefab, contentPanel);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts != null && texts.Length >= 1)
            {
                texts[0].text = "Everything's done!"; 
                if (texts.Length >= 2)
                {
                    texts[1].text = "Get back to Hab and end your sol in the terminal.";
                }
            }
        }
        else
        {
            // vytovreni jedine instance prefabu pro quest s nejnizsim ID
            GameObject entry = Instantiate(questEntryPrefab, contentPanel);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts != null && texts.Length >= 2)
            {
                texts[0].text = questToShow.questName;
                texts[1].text = questToShow.questDescription;
            }
            else
            {
                Debug.LogWarning("Quest prefab neobsahuje požadované textové komponenty (minimálně 2 TextMeshProUGUI).");
            }
        }
    }
}