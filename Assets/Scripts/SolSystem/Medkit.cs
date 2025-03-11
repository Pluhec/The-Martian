using UnityEngine;

public class MedkitTrigger : MonoBehaviour
{
    public Quest questToComplete;
    public QuestTablet questTablet; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && questToComplete != null)
        {
            if (!questToComplete.isCompleted)
            {
                questToComplete.isCompleted = true;
                Debug.Log("Quest byl dokončen přímo pomocí reference!");
            }
            if (questTablet != null)
            {
                questTablet.UpdateQuestList();
            }
        }
    }
}