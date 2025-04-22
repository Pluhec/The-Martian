using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public int mainSlotIndex;
    [HideInInspector] public int slotSize = 1;

    // teď PRIVATE, nepřetahuješ je v inspektoru
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public StorageContainer storageContainer;

    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // vždycky bezpečně najdeme singleton
        inventory = Inventory.Instance;
        // canvas najdeme na scéně jednou (tvůj hlavní UI canvas)
        canvas = FindObjectOfType<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // volá se z Inventory.AddItem / StorageContainer.AddItem
    public void Initialize(int slotIdx, int size, Inventory inv, StorageContainer cont)
    {
        mainSlotIndex = slotIdx;
        slotSize = size;
        inventory = inv;
        storageContainer = cont;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right)
            DropItem();
    }

    private void DropItem()
    {
        if (inventory == null || DroppedItemManager.Instance == null) return;
        var player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;
        var dropPos = (Vector2)player.position + Vector2.up * 0.35f;

        // Pro každý slot části itemu
        for (int k = 0; k < slotSize; k++)
        {
            int idx = mainSlotIndex + k;
            Transform slotTf = null;
            if (inventory != null && idx < inventory.slots.Length)
                slotTf = inventory.slots[idx].transform;
            else if (storageContainer != null && idx < storageContainer.slots.Length)
                slotTf = storageContainer.slots[idx].transform;
            if (slotTf == null || slotTf.childCount == 0) continue;

            var child = slotTf.GetChild(0).gameObject;
            if (k == 0 && child.TryGetComponent<Spawn>(out var sp))
            {
                var dropped = Instantiate(sp.item, dropPos, Quaternion.identity);
                DroppedItemManager.Instance.AddDroppedItem(sp.item, dropPos);
            }

            Destroy(child);
            if (inventory != null) inventory.isFull[idx] = false;
            else storageContainer.isFull[idx] = false;
        }

        inventory?.AlignItems();
        storageContainer?.AlignItems();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e) => transform.position = e.position;

    public void OnEndDrag(PointerEventData e)
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }
}
