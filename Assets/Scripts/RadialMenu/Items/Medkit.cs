using UnityEngine;
using UnityEngine.UI;

public class Medkit : InteractableObject
{
    private Inventory inventory;
    public GameObject itemButton;
    public int slotSize = 2;

    public string itemName = "Neznámý předmět";
    public int questID;

    private QuestManager questManager;
    private QuestTablet questTablet;

    void Awake()
    {
        // Inicializace inventáře
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance missing!");
            return;
        }
        inventory = Inventory.Instance;

        // Inicializace QuestManager a QuestTablet
        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("QuestManager instance not found.");
        }

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
        {
            Debug.LogError("QuestTablet instance not found.");
        }

        actions.Add("Pick Up");
        actions.Add("Heal");
    }

    public override void PerformAction(string action)
    {
        if (action == "Pick Up")
        {
            if (inventory.slots == null) return;

            // Bezpečný průchod sloty
            for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
            {
                bool isSpaceFree = true;

                // Kontrola volných slotů
                for (int j = 0; j < slotSize; j++)
                {
                    if (i + j >= inventory.slots.Length || inventory.isFull[i + j])
                    {
                        isSpaceFree = false;
                        break;
                    }
                }

                if (!isSpaceFree) continue;

                // Vytvoření hlavního tlačítka
                GameObject mainItem = Instantiate(itemButton, inventory.slots[i].transform, false);
                inventory.isFull[i] = true;

                ItemButton itemScript = mainItem.GetComponent<ItemButton>();
                if (itemScript == null) continue;

                itemScript.mainSlotIndex = i;
                itemScript.slotSize = slotSize;

                // Vytvoření placeholderů
                for (int j = 1; j < slotSize; j++)
                {
                    int targetIndex = i + j;
                    if (targetIndex >= inventory.slots.Length) break;

                    GameObject placeholder = Instantiate(itemButton, inventory.slots[targetIndex].transform, false);

                    // Zneškodnění placeholdera
                    Destroy(placeholder.GetComponent<ItemButton>());
                    Destroy(placeholder.GetComponent<Button>());

                    Image img = placeholder.GetComponent<Image>();
                    if (img != null) img.color = new Color(1, 1, 1, 0.35f);

                    placeholder.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                    inventory.isFull[targetIndex] = true;
                }

                GetComponent<PersistentItem>()?.MarkCollected();

                // Odstranění ze světa
                if (DroppedItemManager.Instance != null)
                    DroppedItemManager.Instance.RemoveDroppedItem(gameObject);

                Destroy(gameObject);
                break;
            }
        }

        if (action == "Heal")
        {
            Debug.Log("Heal");

            if (questManager == null)
            {
                Debug.LogError("QuestManager is not initialized.");
                return;
            }

            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
            if (quest != null)
            {
                if (!quest.isCompleted)
                {
                    Debug.Log("Before update: Quest " + quest.questName + " (ID: " + quest.questID + ") isCompleted: " + quest.isCompleted);

                    questManager.MarkQuestAsCompletedByID(questID);

                    Debug.Log("After update: Quest " + quest.questName + " (ID: " + quest.questID + ") isCompleted: " + quest.isCompleted);
                }
                else
                {
                    Debug.Log("Quest " + quest.questName + " is already completed.");
                }

                if (questTablet != null)
                {
                    questTablet.UpdateQuestList();
                }
                else
                {
                    Debug.LogError("QuestTablet is not initialized.");
                }
            }
            else
            {
                Debug.LogWarning("Quest with ID " + questID + " was not found in active quests.");
            }
            
            Destroy(gameObject);
        }
    }
}