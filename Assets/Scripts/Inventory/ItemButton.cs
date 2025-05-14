using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemID = "";

    [HideInInspector] public InteractableObject sourceObject;
    [HideInInspector] public int mainSlotIndex;
    [HideInInspector] public int slotSize = 1;

    [HideInInspector] public Inventory inventory;
    [HideInInspector] public StorageContainer storageContainer;

    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup cg;

    private PlayerInteraction2D playerInteraction;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
            itemID = Guid.NewGuid().ToString();

        inventory = Inventory.Instance;
        canvas = FindObjectOfType<Canvas>();
        cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        playerInteraction = FindObjectOfType<PlayerInteraction2D>();
        if (playerInteraction == null)
            Debug.LogError("PlayerInteraction2D instance not found!");
    }

    public void Initialize(int slotIdx, int size, Inventory inv, StorageContainer cont)
    {
        mainSlotIndex = slotIdx;
        slotSize = size;
        inventory = inv;
        storageContainer = cont;
        originalParent = transform.parent;
    }

    void RemoveOwnPlaceholders()
    {
        for (int j = 0; j < slotSize; j++)
        {
            int idx = mainSlotIndex + j;

            if (inventory != null && idx < inventory.slots.Length)
            {
                var tf = inventory.slots[idx].transform;
                if (tf.childCount > 0)
                {
                    var ph = tf.GetChild(0).GetComponent<ItemPlaceholder>();
                    if (ph != null && ph.mainSlotIndex == mainSlotIndex)
                        Destroy(tf.GetChild(0).gameObject);
                }
                inventory.isFull[idx] = false;
            }

            if (storageContainer != null && idx < storageContainer.slots.Length)
            {
                var tf = storageContainer.slots[idx].transform;
                if (tf.childCount > 0)
                {
                    var ph = tf.GetChild(0).GetComponent<ItemPlaceholder>();
                    if (ph != null && ph.mainSlotIndex == mainSlotIndex)
                        Destroy(tf.GetChild(0).gameObject);
                }
                storageContainer.isFull[idx] = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        RemoveOwnPlaceholders();
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e) => transform.position = e.position;

    public void OnEndDrag(PointerEventData e)
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        cg.blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
            DropItem();
        else if (e.button == PointerEventData.InputButton.Left)
            UseItem();
    }

    private void UseItem()
    {
        if (sourceObject == null)
        {
            Debug.LogWarning("sourceObject není nastavený.");
            return;
        }

        var actions = sourceObject.GetActions();
        if (actions.Count == 0)
        {
            Debug.LogWarning("Tento item nemá definovanou žádnou akci.");
            return;
        }

        // 1️⃣ Spusť akci
        sourceObject.PerformAction(actions[0]);

        // 2️⃣ Nenič tlačítko, pokud je to nástroj
        if (actions[0] == "Use")   // lopata, sekera…  nech v inventáři
            return;

        // 3️⃣ Spotřební věci (léky, jídlo…) smaž jako dřív
        for (int i = 0; i < slotSize; i++)
        {
            int index = mainSlotIndex + i;
            if (index >= inventory.slots.Length) break;

            Transform slot = inventory.slots[index].transform;
            if (slot.childCount == 0) continue;

            GameObject child = slot.GetChild(0).gameObject;
            Destroy(child);
            inventory.isFull[index] = false;
        }
    }


    void DropItem()
{
    if (inventory == null || inventory.slots == null) return;
    if (DroppedItemManager.Instance == null) return;

    Transform player = GameObject.FindGameObjectWithTag("Player").transform;
    Vector2 dropPosition = new Vector2(player.position.x, player.position.y + 0.35f);

    if (TryGetComponent<ContainerSpawn>(out var csp))
    {
        Vector2 dropPos = (Vector2)player.position + Vector2.up * 0.35f;
        csp.SpawnContainer(dropPos);

        for (int j = 0; j < slotSize; j++)
        {
            int idx = mainSlotIndex + j;
            if (idx >= inventory.slots.Length) continue;

            Transform slotTransform = inventory.slots[idx].transform;
            for (int k = slotTransform.childCount - 1; k >= 0; k--)
            {
                var child = slotTransform.GetChild(k).gameObject;
                var ph = child.GetComponent<ItemPlaceholder>();
                if (ph != null && ph.mainSlotIndex == mainSlotIndex)
                {
                    Destroy(child);
                }
            }

            inventory.isFull[idx] = false;
        }

        inventory.AlignItems();
        Destroy(gameObject);
    }

    for (int i = 0; i < slotSize; i++)
    {
        int index = mainSlotIndex + i;
        if (index >= inventory.slots.Length) break;

        Transform slot = inventory.slots[index].transform;
        if (slot.childCount == 0) continue;

        GameObject child = slot.GetChild(0).gameObject;
        if (i == 0 && child.TryGetComponent<Spawn>(out var spawn))
        { 
            // Kontrola, zda se jedná o hlínu (sand)
            bool isSand = false;
            var itemDef = spawn.GetComponent<ItemDefinition>();
            if (itemDef != null && itemDef.itemID == "Dirt")
            {
                isSand = true;
            }

            // Kontrola zakázané oblasti (např. HabArea)
            LayerMask forbiddenZone = LayerMask.GetMask("NoDropZone");
            Collider2D overlap = Physics2D.OverlapPoint(dropPosition, forbiddenZone);

            // Pokud to není hlína a je v zakázané zóně, zabráníme dropnutí
            if (overlap != null && !isSand)
            {
                Debug.Log("❌ Nelze položit item – oblast zakázaná.");
                return; // Zruší drop
            }

            // Pokračuj s běžným dropem
            GameObject spawnedItem = Instantiate(spawn.item, dropPosition, Quaternion.identity);
            DroppedItemManager.Instance.AddDroppedItem(spawn.item, dropPosition);
        }

        Destroy(child);
        inventory.isFull[index] = false;
    }
}
}
