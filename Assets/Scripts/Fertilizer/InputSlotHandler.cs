using UnityEngine;
using UnityEngine.EventSystems;

public class InputSlotHandler : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [HideInInspector] public FertilizerStationController station;
    GameObject currentIcon;

    // 1) Když myš vkročí nad slot
    public void OnPointerEnter(PointerEventData e)
    {
        Debug.Log("[InputSlot] OnPointerEnter, pointer pos = " + e.position);
    }

    // 2) Když myš opustí slot
    public void OnPointerExit(PointerEventData e)
    {
        Debug.Log("[InputSlot] OnPointerExit");
    }

    // 3) Když cokoliv klikneš na slot
    public void OnPointerClick(PointerEventData e)
    {
        Debug.Log($"[InputSlot] OnPointerClick: button = {e.button}");
    }

    // 4) Když sem pustíš drag&drop
    public void OnDrop(PointerEventData e)
    {
        Debug.Log($"[InputSlot] OnDrop called, pointerDrag = {e.pointerDrag?.name}");

        var go = e.pointerDrag;
        if (go == null)
        {
            Debug.LogWarning("[InputSlot] pointerDrag is null");
            return;
        }

        var btn = go.GetComponent<ItemButton>();
        if (btn == null)
        {
            Debug.LogWarning($"[InputSlot] Dropped object {go.name} has no ItemButton");
            return;
        }

        Debug.Log($"[InputSlot] Dropped itemID = {btn.itemID}, expected = {station.shitPackID}");
        if (btn.itemID != station.shitPackID)
        {
            Debug.LogWarning("[InputSlot] itemID mismatch → not a ShitPack");
            return;
        }

        Debug.Log("[InputSlot] Removing item from real inventory");
    
        // Nejprve fyzicky odstraníme původní objekt z inventáře
        for (int i = 0; i < btn.slotSize; i++)
        {
            int slotIndex = btn.mainSlotIndex + i;
            if (slotIndex < Inventory.Instance.slots.Length)
            {
                Transform slotTransform = Inventory.Instance.slots[slotIndex].transform;
                // Procházíme všechny potomky slotu a mažeme je
                for (int j = slotTransform.childCount - 1; j >= 0; j--)
                {
                    Destroy(slotTransform.GetChild(j).gameObject);
                }
                Inventory.Instance.isFull[slotIndex] = false;
            }
        }
    
        // Aktualizujeme inventář
        Inventory.Instance.AlignItems();

        Debug.Log("[InputSlot] Fixing icon into inputSlot");
        currentIcon = go;
        currentIcon.transform.SetParent(transform, false);
        currentIcon.transform.localPosition = Vector3.zero;
        currentIcon.transform.localRotation = Quaternion.identity;
        currentIcon.transform.localScale = Vector3.one;

        var cg = currentIcon.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.blocksRaycasts = false;

        station.OnShitPackReceived(btn);
    }

    // 5) Vyčistí slot
    public void Clear()
    {
        Debug.Log("[InputSlot] Clear called");
        if (currentIcon != null)
            Destroy(currentIcon);
        currentIcon = null;
    }
}