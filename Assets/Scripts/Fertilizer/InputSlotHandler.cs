using UnityEngine;
using UnityEngine.EventSystems;

public class InputSlotHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;
    GameObject currentIcon;

    public void OnDrop(PointerEventData e)
    {
        Debug.Log($"[InputSlot] OnDrop called, pointerDrag = {e.pointerDrag?.name}");
        var go = e.pointerDrag;
        if (go == null) return;

        var btn = go.GetComponent<ItemButton>();
        var def = go.GetComponent<ItemDefinition>();
        if (btn == null || def == null)
        {
            Debug.LogWarning("[InputSlot] Dropped object není ItemButton+ItemDefinition");
            return;
        }

        Debug.Log($"[InputSlot] Dropped definition ID = {def.itemID}, expected = {station.shitPackID}");
        if (def.itemID != station.shitPackID)
        {
            Debug.LogWarning("[InputSlot] Není to ShitPack, abort");
            return;
        }

        // 1) Vytvoříme kopii ikonky a přidáme ji pod input
        Debug.Log("[InputSlot] Instantiating new icon under input slot");
        currentIcon = Instantiate(go, transform);
        currentIcon.name = go.name; // hezčí pojmenování

        // Reset lokální RectTransform na (0,0)
        var rt = currentIcon.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation    = Quaternion.identity;
            rt.localScale       = Vector3.one;
        }

        // 2) **Nezablokujeme** raycast – aby šla ikona znovu draggovat
        var cgNew = currentIcon.GetComponent<CanvasGroup>();
        if (cgNew == null) cgNew = currentIcon.AddComponent<CanvasGroup>();
        cgNew.blocksRaycasts = true;
        cgNew.interactable   = true;

        // 3) Odeber originál z inventáře (placeholdery i originální tlačítko)
        Debug.Log($"[InputSlot] Removing original from inventory at {btn.mainSlotIndex}");
        Inventory.Instance.RemoveItem(btn.mainSlotIndex, btn.slotSize);
        Debug.Log("[InputSlot] Inventory.RemoveItem called");

        // 4) Znič originální drag-GO
        Debug.Log("[InputSlot] Destroying original dragged icon");
        Destroy(go);

        // 5) Notifikujeme stanici
        station.OnShitPackReceived(btn);
    }

    public void Clear()
    {
        Debug.Log("[InputSlot] Clear called");
        if (currentIcon != null) Destroy(currentIcon);
        currentIcon = null;
    }
}