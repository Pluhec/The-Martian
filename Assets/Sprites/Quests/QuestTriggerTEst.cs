using UnityEngine;

public class QuestTriggerTEst : MonoBehaviour
{
    public int questID;            
    public Transform myTransform;  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QuestManager.Instance.NotifyTargetReached(questID, myTransform);
        }
    }
}