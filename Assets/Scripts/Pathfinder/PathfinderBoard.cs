using UnityEngine;
using System.Collections.Generic;

public class PathfinderBoard : MonoBehaviour
{
    [Header("Nastavení")]
    public GameObject[] signObjects;   // Pole GameObjectů s cedulemi
    public int questIDToComplete;      // Quest, který se splní po umístění všech cedulí

    private Dictionary<GameObject, bool> placedSigns; // Sledování umístěných cedulí
    private QuestManager questManager;
    private QuestTablet questTablet;
    private Collider2D boardCollider;
    private HashSet<Collider2D> processedColliders = new HashSet<Collider2D>();
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] results = new Collider2D[10]; // Maximální počet výsledků

    private void Start()
    {
        questManager = GameManager.Instance.QuestManager;
        questTablet = FindObjectOfType<QuestTablet>();
        boardCollider = GetComponent<Collider2D>();

        placedSigns = new Dictionary<GameObject, bool>();

        // Inicializuj slovník a vypni všechny cedule na začátku
        foreach (var sign in signObjects)
        {
            if (sign != null)
            {
                sign.SetActive(false);
                placedSigns[sign] = false;
            }
        }
        
        // Nastavení filtru - detekovat všechny collidery
        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(Physics2D.AllLayers);
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        // Kontrola objektů v collideru
        if (boardCollider != null)
        {
            int count = boardCollider.Overlap(contactFilter, results);
            for (int i = 0; i < count; i++)
            {
                var col = results[i];
                if (col != null && col.gameObject != gameObject && !processedColliders.Contains(col))
                {
                    ProcessCollision(col);
                    processedColliders.Add(col);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    private void ProcessCollision(Collider2D collision)
    {
        if (collision.gameObject == gameObject)
            return;

        Debug.Log($"Zpracovávám objekt: {collision.gameObject.name}");

        string itemName = collision.gameObject.name;
        // Odstranit "(Clone)" z názvu instancovaných objektů
        if (itemName.Contains("(Clone)"))
            itemName = itemName.Replace("(Clone)", "").Trim();

        // Hledej odpovídající ceduli podle názvu
        foreach (var sign in signObjects)
        {
            if (sign == null || placedSigns[sign])
                continue;

            // Porovnej jméno herního objektu s názvem kolizního předmětu
            if (sign.name == itemName)
            {
                sign.SetActive(true);
                placedSigns[sign] = true;
                Debug.Log($"Cedule '{sign.name}' byla úspěšně umístěna.");

                // Zničíme položený předmět
                Destroy(collision.gameObject);

                // Kontrola, zda jsou všechny cedule umístěny
                CheckAllSignsPlaced();
                return;
            }
        }
    }

    private void CheckAllSignsPlaced()
    {
        // Zkontroluj, zda jsou všechny cedule umístěny
        bool allPlaced = true;
        foreach (var sign in signObjects)
        {
            if (sign != null && !placedSigns[sign])
            {
                allPlaced = false;
                break;
            }
        }

        if (allPlaced)
        {
            Debug.Log("Všechny cedule jsou umístěny. Quest splněn.");

            // Najdeme a dokončíme questu
            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questIDToComplete);
            if (quest != null && !quest.isCompleted)
            {
                Debug.Log($"Před dokončením: Quest {quest.questName} (ID {questIDToComplete}) isCompleted? {quest.isCompleted}");
                questManager.MarkQuestAsCompletedByID(questIDToComplete);
                Debug.Log($"Po dokončení: Quest {quest.questName} isCompleted? {quest.isCompleted}");
            }

            // Aktualizujeme UI questu
            if (questTablet != null)
                questTablet.UpdateQuestList();

            // Vypneme jen collider a skript místo celého objektu
            if (boardCollider != null)
                boardCollider.enabled = false;

            enabled = false; // Vypne tento skript
        }
    }
}