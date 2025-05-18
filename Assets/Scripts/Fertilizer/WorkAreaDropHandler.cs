using UnityEngine;
using UnityEngine.EventSystems;

public class WorkAreaDropHandler : MonoBehaviour, IDropHandler
{
    [HideInInspector] public FertilizerStationController station;
    public void OnDrop(PointerEventData e)
    {
        // přijmeme pouze ikonu z InputSlotu (během drag&drop se blokuje raycast, proto ověříme tag)
        var go = e.pointerDrag;
        if (go == null) return;

        // stačí jakýkoli go z InputSlotu, protože tam blokujeme raycasts
        station.OnWorkAreaDrop(go);
    }
}