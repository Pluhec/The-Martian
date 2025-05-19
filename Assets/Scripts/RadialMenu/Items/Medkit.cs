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
            Debug.LogError("QuestManager instance not found.");

        questTablet = FindObjectOfType<QuestTablet>();
        if (questTablet == null)
            Debug.LogError("QuestTablet instance not found.");

        // Přidáme akce pro tento objekt:
        // první akce (Heal) se použije při quick‐action,
        // druhá (Pick Up) se objeví v radiálním menu.
        actions.Add("Heal");
        actions.Add("Pick Up");
    }

    public override void PerformAction(string action)
    {
        if (action == "Pick Up")
        {
            if (inventory.slots == null)
                return;

            // Najdeme první volný blok o velikosti slotSize
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

                // 1) Vytvoříme hlavní tlačítko v UI
                GameObject mainItem = Instantiate(itemButton, inventory.slots[i].transform, false);
                inventory.isFull[i] = true;

                var itemScript = mainItem.GetComponent<ItemButton>();
                if (itemScript != null)
                {
                    itemScript.mainSlotIndex = i;
                    itemScript.slotSize      = slotSize;
                    itemScript.sourceObject  = this;
                }

                // 2) Vytvoříme placeholdery pro zbylá políčka
                for (int j = 1; j < slotSize; j++)
                {
                    int targetIndex = i + j;
                    if (targetIndex >= inventory.slots.Length) break;

                    GameObject placeholder = Instantiate(itemButton, inventory.slots[targetIndex].transform, false);

                    Destroy(placeholder.GetComponent<ItemButton>());
                    Destroy(placeholder.GetComponent<Button>());

                    var img = placeholder.GetComponent<Image>();
                    if (img != null)
                        img.color         = new Color(1, 1, 1, 0.35f);

                    placeholder.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                    inventory.isFull[targetIndex] = true;
                }

                // 3) Označíme jako sebráno a odstraníme z kódu světa
                GetComponent<PersistentItem>()?.MarkCollected();
                DroppedItemManager.Instance?.RemoveDroppedItem(gameObject);

                // 4) Deaktivujeme světový objekt, ale necháme ho pro quick‐action z inventáře
                gameObject.SetActive(false);
                return;
            }
        }
        else if (action == "Heal")
        {
            // Quick‐action (E) i použití z inventáře
            Debug.Log("Medkit: Heal");

            if (questManager == null)
            {
                Debug.LogError("QuestManager is not initialized.");
                return;
            }

            // Najdeme a dokončíme questu
            Quest quest = questManager.ActiveQuests.Find(q => q.questID == questID);
            if (quest != null && !quest.isCompleted)
            {
                Debug.Log($"Before heal: Quest {quest.questName} (ID {quest.questID}) isCompleted? {quest.isCompleted}");
                questManager.MarkQuestAsCompletedByID(questID);
                Debug.Log($" After heal: Quest {quest.questName} isCompleted? {quest.isCompleted}");
            }

            // Aktualizujeme UI questu
            if (questTablet != null)
                questTablet.UpdateQuestList();
            
            PlayerPrefs.SetInt("BloodOverlayDisabled", 1);
            PlayerPrefs.Save();
            bloodOverlayImage.enabled = false;

            // 5) Odstraníme světový objekt (pokud byl deaktivovaný, znova ho zničíme)
            Destroy(gameObject);
        }
    }
    
}