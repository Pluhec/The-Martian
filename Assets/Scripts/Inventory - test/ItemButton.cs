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
        // Kontrola existence závislostí
        if (inventory == null || inventory.slots == null) return;
        if (DroppedItemManager.Instance == null) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 dropPosition = new Vector2(player.position.x, player.position.y + 0.35f);

        for (int i = 0; i < slotSize; i++)
        {
            int index = mainSlotIndex + i;
            
            // Kontrola platnosti indexu
            if (index >= inventory.slots.Length) break;

            Transform slot = inventory.slots[index].transform;
            if (slot.childCount == 0) continue;

            GameObject child = slot.GetChild(0).gameObject;
            
            // Hlavní item spawnuje fyzický objekt
            if (i == 0 && child.TryGetComponent<Spawn>(out var spawn))
            {
                GameObject spawnedItem = Instantiate(spawn.item, dropPosition, Quaternion.identity);
                DroppedItemManager.Instance.AddDroppedItem(spawn.item, dropPosition);
            }

            Destroy(child);
            inventory.isFull[index] = false;
        }
    }
}
