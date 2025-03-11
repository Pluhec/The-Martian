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
        // smazani stareho questu z listu
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }
}