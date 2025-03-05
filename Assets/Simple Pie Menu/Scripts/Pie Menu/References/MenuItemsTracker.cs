using System.Collections.Generic;
using UnityEngine;

namespace SimplePieMenu
{
    public class MenuItemsTracker : MonoBehaviour
    {
        public Dictionary<int, PieMenuItem> PieMenuItems { get; private set; }
        public Dictionary<int, Button> ButtonComponents { get; private set; }

        public void Initialize(Transform menuItemsDir)
        {
            // Implementation
        }

        public void InitializeMenuItem(Transform menuItem, int index)
        {
            // Implementation
        }
    }
}