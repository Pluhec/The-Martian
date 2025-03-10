using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTablet : MonoBehaviour
{
    [Header("Reference na QuestManager")]
    public QuestManager questManager;
    
    [Header("UI komponenty")]
    // Prefab pro zobrazení jednoho questu – obsahuje UI prvky pro název, popis a stav
    public GameObject questEntryPrefab;
    // Panel (např. obsah ScrollView), kam se budou instance questEntryPrefab vkládat
    public Transform contentPanel;

    // Metoda, která načte a zobrazí seznam questů
    public void UpdateQuestList()
    {
        // Nejprve smažeme staré záznamy
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Projdeme všechny questy z QuestManageru a vytvoříme záznam
        foreach (Quest quest in questManager.activeQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, contentPanel);
            QuestEntryUI entryUI = entry.GetComponent<QuestEntryUI>();
            if (entryUI != null)
            {
                entryUI.Setup(quest);
            }
        }
    }
}