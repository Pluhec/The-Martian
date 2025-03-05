using System;
using UnityEngine;
using SimplePieMenu;

public class DynamicClickHandler : MonoBehaviour, IMenuItemClickHandler
{
    private Action onClickAction;

    public void SetAction(Action action)
    {
        onClickAction = action;
    }

    // Tato metoda bude volána, když uživatel klikne na položku menu.
    public void Handle()
    {
        onClickAction?.Invoke();
    }
}