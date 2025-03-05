using UnityEngine;
using SimplePieMenu; // Přidáno, pokud komponenta MenuItemSelector je v tomto jmenném prostoru

namespace SimplePieMenu
{
    public class MenuItemSelectionHandler : MonoBehaviour
    {
        private MenuItemSelector menuItemSelector;

        private void Start()
        {
            menuItemSelector = GetComponent<MenuItemSelector>();
            if (menuItemSelector == null)
            {
                Debug.LogError("MenuItemSelector component not found.");
            }
        }
    }
}