using UnityEngine;
using UnityEngine.EventSystems;

public class InputSlotHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;
    GameObject currentIcon;

    public void OnDrop(PointerEventData e)
    {
        if (station.hasPackInInput) return;

        var go = e.pointerDrag;
        if (go == null) return;

        var btn = go.GetComponent<ItemButton>();
        var def = go.GetComponent<ItemDefinition>();
        if (btn == null || def == null) return;
        if (def.itemID != station.shitPackID) return;
        
        currentIcon = Instantiate(go, transform);
        currentIcon.name = go.name;
        var rt = currentIcon.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation    = Quaternion.identity;
            rt.localScale       = Vector3.one;
        }
        var cg = currentIcon.GetComponent<CanvasGroup>() ?? currentIcon.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = true;
        cg.interactable   = true;
        
        Inventory.Instance.RemoveItem(btn.mainSlotIndex, btn.slotSize);
        
        Destroy(go);
        
        station.OnShitPackReceived(btn);
    }

    public void Clear()
    {
        if (currentIcon != null) Destroy(currentIcon);
        currentIcon = null;
    }
}