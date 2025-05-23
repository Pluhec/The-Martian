using System;
using System.Linq;
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

    public static bool isDragEnabled = false;

    private GameObject toastPrefab;
    private Transform notificationsParent;

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

        RefreshNotificationSystem();
    }

    private void RefreshNotificationSystem()
    {
        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }
    }

    private void ShowToast(string type, string message)
    {
        RefreshNotificationSystem();
        if (toastPrefab != null && notificationsParent != null)
        {
            var toast = Instantiate(toastPrefab, notificationsParent);
            toast.GetComponent<Toast>()?.Show(type, message);
        }
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
        if (!isDragEnabled)
        {
            e.Use();
            return;
        }

        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        RemoveOwnPlaceholders();
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!isDragEnabled)
        {
            e.Use();
            return;
        }

        transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!isDragEnabled)
        {
            e.Use();
            return;
        }

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
        if (gameObject.name.Contains("Shovel"))
        {
            ShovelLogic.Dig();
            return;
        }

        if (gameObject.name.Contains("Shovel"))
        {
            Debug.LogError("Nelze najít lopatu (ani neaktivní)!");
            return;
        }

        var actions = sourceObject.GetActions();
        if (actions.Count == 0)
        {
            Debug.LogWarning("Tento item nemá definovanou žádnou akci.");
            return;
        }

        sourceObject.PerformAction(actions[0]);

        if (actions[0] == "Use")
            return;

        for (int i = 0; i < slotSize; i++)
        {
            int index = mainSlotIndex + i;
            if (index >= inventory.slots.Length) break;

            Transform slot = inventory.slots[index].transform;
            if (slot.childCount == 0) continue;

            if (sourceObject is Medkit)
            {
                GameObject child = slot.GetChild(0).gameObject;
                Destroy(child);
                inventory.isFull[index] = false;
            }
        }
    }

    public void DropItem()
    {
        if (inventory == null || inventory.slots == null) return;
        if (DroppedItemManager.Instance == null) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 dropPosition = new Vector2(player.position.x, player.position.y + 0.35f);

        LayerMask forbiddenZone = LayerMask.GetMask("NoDropZone");
        Collider2D overlap = Physics2D.OverlapPoint(dropPosition, forbiddenZone);

        if (TryGetComponent<ContainerSpawn>(out var csp))
        {
            if (overlap != null)
            {
                ShowToast("warning", "You cannot drop items here!");
                return;
            }

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
                bool isSand = false;
                var itemDef = spawn.GetComponent<ItemDefinition>();
                if (itemDef != null && itemDef.itemID == "Dirt" || itemDef != null && itemDef.itemID == "HydrazineBarel")
                {
                    isSand = true;
                }

                if (overlap != null && !isSand)
                {
                    ShowToast("warning", "You cannot drop items here!");
                    return;
                }

                GameObject spawnedItem = Instantiate(spawn.item, dropPosition, Quaternion.identity);
                DroppedItemManager.Instance.AddDroppedItem(spawn.item, dropPosition);
            }

            Destroy(child);
            inventory.isFull[index] = false;
        }
    }
}