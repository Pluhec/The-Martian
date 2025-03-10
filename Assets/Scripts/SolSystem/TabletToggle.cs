using UnityEngine;
using UnityEngine.UI;

public class TabletToggle : MonoBehaviour
{
    [Header("UI prvky")]
    public GameObject closedTablet;
    public GameObject openTablet;
    
    [Header("Quest Tablet")]
    public QuestTablet questTablet;
    
    private bool isOpen = false;
    
    public void ToggleTablet()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            
            closedTablet.SetActive(false);
            openTablet.SetActive(true);

            // Aktualizace questů – zobrazí se pouze ty, které ještě nejsou dokončeny
            if (questTablet != null)
            {
                questTablet.UpdateQuestList();
            }
        } 
        else
        {
            // Při zavření vrátíme aktivní pouze uzavřenou verzi
            closedTablet.SetActive(true);
            openTablet.SetActive(false);
        }
    }
}