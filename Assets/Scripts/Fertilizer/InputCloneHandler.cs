// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class InputCloneHandler : MonoBehaviour, IBeginDragHandler
// {
//     [HideInInspector] public InputSlotHandler slotHandler;
//
//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         Debug.Log("[InputClone] OnBeginDrag → clearing input slot");
//         slotHandler.Clear();
//     }
// }