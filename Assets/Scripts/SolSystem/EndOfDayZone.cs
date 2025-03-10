using UnityEngine;

public class EndOfDayZone : MonoBehaviour
{
    [Header("Reference na další systémy")]
    public QuestManager questManager;
    public SolSystem solSystem;

    // Použijeme OnTriggerEnter2D, jelikož se jedná o 2D hru
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ověříme, že objekt, který vstoupil do triggeru, je hráč (tag "Player")
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