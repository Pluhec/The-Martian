using UnityEngine;
using TMPro;

public class TabletToggle : MonoBehaviour
{
    [Header("Odkazy na UI")]
    public GameObject closedTablet;
    public GameObject openedTablet;

    [Header("Odkazy na text")]
    public TextMeshProUGUI solNumberText;

    [Header("Odkazy na logiku")]
    public SolSystem solSystem;
    public QuestTablet questTablet;

    private bool isOpen = false;

    public void ToggleTablet()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            openedTablet.SetActive(true);
            closedTablet.SetActive(false);
            
            if (solNumberText != null && solSystem != null)
            {
                solNumberText.text = "<rotate=90>SOL " + solSystem.currentSol;
            }
            
            if (questTablet != null)
            {
                questTablet.UpdateQuestList();
            }
        }
        else
        {
            openedTablet.SetActive(false);
            closedTablet.SetActive(true);
        }
    }
}