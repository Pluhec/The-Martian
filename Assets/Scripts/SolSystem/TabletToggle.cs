using UnityEngine;
using TMPro;

public class TabletToggle : MonoBehaviour
{
    [Header("Button")]
    public GameObject closedTablet;
    public GameObject openedTablet;

    [Header("Tablet texty")]
    public TextMeshProUGUI solNumberText;
    public TextMeshProUGUI questHeaderText;
    public TextMeshProUGUI questDescriptionText;

    [Header("Logika")]
    public SolSystem solSystem;
    public QuestManager questManager;
    
    private bool isOpen = false;
    
    public void ToggleTablet()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            openedTablet.SetActive(true);
            closedTablet.SetActive(false);
            
            UpdateTabletUI();
        }
        else
        {
            openedTablet.SetActive(false);
            closedTablet.SetActive(true);
        }
    }

    private void UpdateTabletUI()
    {
        // cislo solu
        if (solNumberText != null && solSystem != null)
        {
            solNumberText.text = "<rotate=90>SOL " + solSystem.currentSol;
        }

        // vyhledani prvniho nedokonceneho questu
        Quest firstIncompleteQuest = null;
        foreach (Quest quest in questManager.activeQuests)
        {
            if (!quest.isCompleted)
            {
                firstIncompleteQuest = quest;
                break;
            }
        }

        // zobrazeni nejblizsiho nedokonceneho questu
        if (firstIncompleteQuest != null)
        {
            if (questHeaderText != null)
            {
                questHeaderText.text = firstIncompleteQuest.questName;
            }
            if (questDescriptionText != null)
            {
                questDescriptionText.text = firstIncompleteQuest.questDescription;
            }
        }
        else
        {
            // zadne ukoly
            if (questHeaderText != null)
            {
                questHeaderText.text = "Vše splněno!";
            }
            if (questDescriptionText != null)
            {
                questDescriptionText.text = "Nemáš žádné aktivní úkoly. Jdi na počítač a ukonči sol";
            }
        }
    }
}