using UnityEngine;
using UnityEngine.EventSystems;

public class WorkAreaDropHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;

    public void OnDrop(PointerEventData e)
    {
        GameObject go = e.pointerDrag;
        if (go == null)
            return;
        
        var btn = go.GetComponent<ItemButton>();
        var def = go.GetComponent<ItemDefinition>();
        if (btn == null || def == null)
        {
            return;
        }
        
        if (def.itemID != station.shitPackID)
        {
            return;
        }
        
        if (!station.hasPackInInput)
        {
            Inventory.Instance.RemoveItem(btn.mainSlotIndex, btn.slotSize);
            Destroy(go);
            station.OnShitPackReceived(btn);
        }
        
        station.OnWorkAreaDrop(go);
    }
}