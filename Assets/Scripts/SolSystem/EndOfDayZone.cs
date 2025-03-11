using UnityEngine;

public class EndOfDayZone : MonoBehaviour
{
    [Header("Reference na další systémy")]
    public QuestManager questManager;
    public SolSystem solSystem;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (questManager.AreAllQuestsCompleted())
            {
                Debug.Log("EndOfDayZone: Všechny questy jsou splněny. Den se ukončuje...");
                solSystem.EndCurrentSol();
            }
            else
            {
                Debug.Log("EndOfDayZone: Nemůžeš ukončit den, dokud nejsou všechny questy dokončeny!");
            }
        }
    }
}