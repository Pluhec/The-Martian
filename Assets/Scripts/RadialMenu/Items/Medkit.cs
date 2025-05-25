using UnityEngine;
using UnityEngine.UI;

public class Medkit : InteractableObject
{
    private Inventory inventory;
    public GameObject itemButton;
    public int slotSize = 2;
    public int questID;
    public Canvas bloodOverlayImage;
    private QuestManager questManager;
    private QuestTablet questTablet;

    void Awake()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance missing!");
            return;
        }
        inventory = Inventory.Instance;

        questManager = QuestManager.Instance;
        if (questManager == null)
            Debug.LogError("QuestManager instance not found.");

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
            Debug.LogError("QuestTablet instance not found.");

        actions.Add("Heal");
        actions.Add("Pick Up");
    }

    public override void PerformAction(string action)
    {
        if (action == "Pick Up")
        {
            if (inventory.slots == null)
                return;

            for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
            {
                bool isSpaceFree = true;
                for (int j = 0; j < slotSize; j++)
                {
                    if (i + j >= inventory.slots.Length || inventory.isFull[i + j])
                    {
                        isSpaceFree = false;
                        break;
                    }
                }
                if (!isSpaceFree) continue;

                GameObject mainItem = Instantiate(itemButton, inventory.slots[i].transform, false);
                inventory.isFull[i] = true;

                var itemScript = mainItem.GetComponent<ItemButton>();
                if (itemScript != null)
                {
                    itemScript.mainSlotIndex = i;
                    itemScript.slotSize = slotSize;
                    itemScript.sourceObject = this;
                }

                for (int j = 1; j < slotSize; j++)
                {
                    int targetIndex = i + j;
                    if (targetIndex >= inventory.slots.Length) break;

                    GameObject placeholder = Instantiate(itemButton, inventory.slots[targetIndex].transform, false);

                    Destroy(placeholder.GetComponent<ItemButton>());
                    Destroy(placeholder.GetComponent<Button>());

                    var img = placeholder.GetComponent<Image>();
                    if (img != null)
                        img.color = new Color(1, 1, 1, 0.35f);

                    placeholder.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                    inventory.isFull[targetIndex] = true;
                }

                GetComponent<PersistentItem>()?.MarkCollected();
                DroppedItemManager.Instance?.RemoveDroppedItem(gameObject);

                gameObject.SetActive(false);
                return;
            }
        }
        else if (action == "Heal")
        {
            Debug.Log("Medkit: Heal");

            if (questManager == null)
            {
                Debug.LogError("QuestManager is not initialized.");
                return;
            }

            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
            if (quest != null && !quest.isCompleted)
            {
                Debug.Log($"Before heal: Quest {quest.questName} (ID {quest.questID}) isCompleted? {quest.isCompleted}");
                questManager.MarkQuestAsCompletedByID(questID);
                Debug.Log($" After heal: Quest {quest.questName} isCompleted? {quest.isCompleted}");
            }

            if (questTablet != null)
                questTablet.UpdateQuestList();

            PlayerPrefs.SetInt("BloodOverlayDisabled", 1);
            PlayerPrefs.Save();
            bloodOverlayImage.enabled = false;

            Destroy(gameObject);
        }
    }
}