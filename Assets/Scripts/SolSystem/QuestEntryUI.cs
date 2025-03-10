using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questStatusText; 
    
    public void Setup(Quest quest)
    {
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.questDescription;
        questStatusText.text = quest.isCompleted ? "<color=#00FF00>Complete</color>" : "<color=#FF0000>Incomplete</color>";
    }
}