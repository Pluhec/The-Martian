using UnityEngine;
using UnityEngine.EventSystems;

public class WorkAreaDropHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;

    public void OnDrop(PointerEventData e)
    {
        Debug.Log($"[WorkArea] OnDrop called, pointerDrag = {e.pointerDrag?.name}");
        var go = e.pointerDrag;
        if (go == null)
        {
            Debug.LogWarning("[WorkArea] pointerDrag is null");
            return;
        }

        Debug.Log("[WorkArea] Accepting drop from InputSlot");
        station.OnWorkAreaDrop(go);
    }
}