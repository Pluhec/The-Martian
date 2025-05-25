using UnityEngine;

public class PathfinderVisibilityCheck : MonoBehaviour
{
    public int questID;
    private bool questCompleted = false;

    private void OnBecameVisible()
    {
        if (!questCompleted)
        {
            Debug.Log($"Objekt {gameObject.name} je nyní viditelný v kameře");
            QuestManager.Instance.MarkQuestAsCompletedByID(questID);
            questCompleted = true;
        }
    }
}