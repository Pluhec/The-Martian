using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemID = "";

    [HideInInspector] public int mainSlotIndex;
    [HideInInspector] public int slotSize = 1;

    [HideInInspector] public Inventory        inventory;
    [HideInInspector] public StorageContainer storageContainer;

    Transform   originalParent;
    Canvas      canvas;
    CanvasGroup cg;

    /*──────────────────────────────*/
    /*  INIT                        */
    /*──────────────────────────────*/
    void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
            itemID = Guid.NewGuid().ToString();

        inventory = Inventory.Instance;
        canvas    = FindObjectOfType<Canvas>();
        cg        = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void Initialize(int slotIdx, int size, Inventory inv, StorageContainer cont)
    {
        mainSlotIndex    = slotIdx;
        slotSize         = size;
        inventory        = inv;
        storageContainer = cont;
        originalParent   = transform.parent;
    }

    /*──────────────────────────────*/
    /*  HELPERS                     */
    /*──────────────────────────────*/
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

    /*──────────────────────────────*/
    /*  DRAG & DROP                 */
    /*──────────────────────────────*/
    public void OnBeginDrag(PointerEventData e)
    {
        /* zruš placeholdery ve starém kontejneru */
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

    /*──────────────────────────────*/
    /*  PRAVÝ KLIK = DROP           */
    /*──────────────────────────────*/
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right) DropItem();
    }

    void DropItem()
    {
        if (DroppedItemManager.Instance == null) return;
        var player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        Vector2 dropPos = (Vector2)player.position + Vector2.up * 0.35f;

        /* zahoď hlavní ikonu + placeholdery */
        RemoveOwnPlaceholders();
        Destroy(gameObject);

        var spawn = GetComponent<Spawn>();
        if (spawn != null)
            DroppedItemManager.Instance.AddDroppedItem(spawn.item, dropPos);

        inventory?.AlignItems();
        storageContainer?.AlignItems();
    }
}
