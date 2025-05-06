using System.Collections.Generic;
using UnityEngine;

public class HabAreaChecker : MonoBehaviour
{
    public int questID;

    private HashSet<Collider2D> objectsInArea = new HashSet<Collider2D>();
    private bool questCompleted = false;
    private QuestManager questManager;
    private QuestTablet questTablet;

    private void Start()
    {
        // Najde QuestManager a QuestTablet pokud nejsou nastavené ručně
        if (questManager == null)
            questManager = FindObjectOfType<QuestManager>();

        if (questTablet == null)
            questTablet = FindObjectOfType<QuestTablet>();

        if (questManager == null)
            Debug.LogError("❌ QuestManager nebyl nalezen ve scéně!");

        if (questTablet == null)
            Debug.LogWarning("⚠️ QuestTablet nebyl nalezen – seznam úkolů se neaktualizuje vizuálně.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            objectsInArea.Add(other);
            Debug.Log($"🔽 Vstup: {other.name}, celkem uvnitř: {objectsInArea.Count}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            objectsInArea.Remove(other);
            Debug.Log($"🔼 Výstup: {other.name}, zůstává: {objectsInArea.Count}");
            CheckIfAreaIsEmpty();
        }
    }

    private void CheckIfAreaIsEmpty()
    {
        if (!questCompleted && objectsInArea.Count == 0)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        if (questManager == null)
        {
            Debug.LogError("❌ Není dostupný QuestManager, nelze dokončit úkol.");
            return;
        }

        Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
        if (quest != null && !quest.isCompleted)
        {
            Debug.Log($"✅ Dokončování úkolu: {quest.questName} (ID {quest.questID})");
            questManager.MarkQuestAsCompletedByID(questID);
            questCompleted = true;
        }

        if (questTablet != null)
            questTablet.UpdateQuestList();
    }
}
