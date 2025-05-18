using UnityEngine;
using UnityEngine.EventSystems;

public class InputSlotHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;
    GameObject currentIcon;

    public void OnDrop(PointerEventData e)
    {
        var go = e.pointerDrag;
        if (go == null) return;
        var btn = go.GetComponent<ItemButton>();
        if (btn == null) return;

        // přijímáme jen ShitPack
        if (btn.itemID != station.shitPackID) return;

        // 1) odstraň ze skutečného inventáře
        Inventory.Instance.RemoveItem(btn.mainSlotIndex, btn.slotSize);

        // 2) zafixuj ikonku v našem slotu
        currentIcon = go;
        currentIcon.transform.SetParent(transform, false);
        currentIcon.GetComponent<CanvasGroup>().blocksRaycasts = false;

        station.OnShitPackReceived(btn);
    }

    // vyčistí slot, vrátí se do prázdného stavu
    public void Clear()
    {
        if (currentIcon != null)
            Destroy(currentIcon);
        currentIcon = null;
    }
}