using UnityEngine;
using UnityEngine.EventSystems;

public class WorkAreaDropHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;

    public void OnDrop(PointerEventData e)
    {
        var go = e.pointerDrag;
        if (go == null) return;
        station.OnWorkAreaDrop(go);
    }
}