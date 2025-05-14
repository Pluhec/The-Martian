using UnityEngine;
using System.Collections.Generic;

public class PathfinderBoard : InteractableObject
{
    [Header("Nastavení")]
    public GameObject[] signObjects;   // Pole GameObjectů s cedulemi
    public int questIDToComplete;      // Quest, který se splní po umístění všech cedulí

    private Dictionary<GameObject, bool> placedSigns; // Sledování umístěných cedulí
    private QuestManager questManager;
    private QuestTablet questTablet;
    private Inventory inventory;

    private void Start()
    {
        questManager = GameManager.Instance.QuestManager;
        inventory = Inventory.Instance;
        questTablet = FindObjectOfType<QuestTablet>();
        
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

        actions.Add("Use");
    }

    public override void PerformAction(string action)
    {
        if (action == "Use")
        {
            bool anyPlaced = false;

            // Projdi všechny sloty inventáře
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                var slot = inventory.slots[i];
                if (slot.transform.childCount > 0)
                {
                    var item = slot.transform.GetChild(0).gameObject;
                    var itemDefinition = item.GetComponent<ItemDefinition>();

                    // Pokud předmět nemá definici, přeskoč
                    if (itemDefinition == null)
                        continue;

                    // Najdi odpovídající ceduli podle itemID
                    foreach (var sign in signObjects)
                    {
                        // Pokud už je cedule umístěna nebo je null, přeskoč ji
                        if (sign == null || placedSigns[sign])
                            continue;

                        // Porovnej jméno herního objektu s itemID
                        if (sign.name == itemDefinition.itemID)
                        {
                            var itemBtn = item.GetComponent<ItemButton>();
                            if (itemBtn != null)
                            {
                                inventory.RemoveItem(i, itemBtn.slotSize);
                                // Resetuj celé pole isFull
                                for (int k = 0; k < inventory.isFull.Length; k++)
                                {
                                    inventory.isFull[k] = false;
                                }
                                sign.SetActive(true);
                                placedSigns[sign] = true;
                                anyPlaced = true;
                                Debug.Log($"Cedule '{sign.name}' byla úspěšně umístěna.");
                                break;  // Přeruš vnitřní smyčku, pokračuj v dalším inventáři
                            }
                        }
                    }
                }
            }

            if (!anyPlaced)
            {
                Debug.Log("Nemáš žádnou potřebnou ceduli v inventáři.");
            }
            else
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
                    
                    // Najdeme a dokončíme questu - podobně jako v Medkit
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
                    Collider2D collider = GetComponent<Collider2D>();
                    if (collider != null)
                        collider.enabled = false;
                    
                    enabled = false; // Vypne tento skript
                }
            }
        }
        else
        {
            base.PerformAction(action);
        }
    }
}