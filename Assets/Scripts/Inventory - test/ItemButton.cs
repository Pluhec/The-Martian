using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler
{
    Slot slot;

    private void Awake()
    {
        slot = GetComponentInParent<Slot>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Levý klik!");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Pravý klik!");
            slot.DropItem();
        }
    }
}
