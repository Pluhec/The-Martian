using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rt;
    CanvasGroup cg;
    OrderManager manager;
    int startIndex;
    
    public ItemType itemType;

    public void Init(OrderManager mgr) {
        manager = mgr;
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        startIndex = manager.currentOrder.IndexOf(this);
        cg.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        rt.anchoredPosition += new Vector2(0, eventData.delta.y);
        for (int i = 0; i < manager.slotTransforms.Count; i++) {
            float slotY = manager.slotTransforms[i].anchoredPosition.y;
            if (rt.anchoredPosition.y < slotY + 20 && rt.anchoredPosition.y > slotY - 20) {
                if (i != startIndex) {
                    manager.InsertAt(this, i);
                    startIndex = i;
                }
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        cg.blocksRaycasts = true;
        rt.anchoredPosition = manager.slotTransforms[startIndex].anchoredPosition;
    }
    
    public void SnapTo(Vector2 targetPos) {
        rt.anchoredPosition = targetPos;
    }
}