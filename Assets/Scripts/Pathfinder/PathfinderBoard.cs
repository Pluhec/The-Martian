using UnityEngine;
using System.Collections.Generic;
public class PathfinderBoard : MonoBehaviour
{
    [Header("Nastavení")] 
    public GameObject[] signObjects;
    public int questIDToComplete;

    private Dictionary<GameObject, bool> placedSigns;
    private QuestManager questManager;
    private QuestTablet questTablet;
    private Collider2D boardCollider;
    private HashSet<Collider2D> processedColliders = new HashSet<Collider2D>();
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] results = new Collider2D[10];

    private void Start()
    {
        questManager = GameManager.Instance.QuestManager;
        questTablet = FindObjectOfType<QuestTablet>();
        boardCollider = GetComponent<Collider2D>();

        placedSigns = new Dictionary<GameObject, bool>();

        foreach (var sign in signObjects)
        {
            if (sign != null)
            {
                sign.SetActive(false);
                placedSigns[sign] = false;
            }
        }

        contactFilter.useTriggers = true;
        contactFilter.SetLayerMask(Physics2D.AllLayers);
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
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

        string itemName = collision.gameObject.name;
        if (itemName.Contains("(Clone)"))
            itemName = itemName.Replace("(Clone)", "").Trim();

        foreach (var sign in signObjects)
        {
            if (sign == null || placedSigns[sign])
                continue;

            if (sign.name == itemName)
            {
                sign.SetActive(true);
                placedSigns[sign] = true;
                Destroy(collision.gameObject);
                CheckAllSignsPlaced();
                return;
            }
        }
    }

    private void CheckAllSignsPlaced()
    {
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
            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questIDToComplete);
            if (quest != null && !quest.isCompleted)
            {
                questManager.MarkQuestAsCompletedByID(questIDToComplete);
            }

            if (questTablet != null)
                questTablet.UpdateQuestList();

            if (boardCollider != null)
                boardCollider.enabled = false;

            enabled = false;
        }
    }
}