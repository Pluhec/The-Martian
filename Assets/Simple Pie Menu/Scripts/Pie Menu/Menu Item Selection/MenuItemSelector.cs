using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimplePieMenu
{
    public class MenuItemsTracker : MonoBehaviour
    {
        public Dictionary<int, Button> ButtonComponents { get; private set; }
        public Dictionary<int, PieMenuItem> PieMenuItems { get; private set; }
        public Dictionary<int, PieMenuItem> HiddenMenuItems { get; private set; }

        private void Awake()
        {
            ButtonComponents = new Dictionary<int, Button>();
            PieMenuItems = new Dictionary<int, PieMenuItem>();
            HiddenMenuItems = new Dictionary<int, PieMenuItem>();
        }

        public void Initialize(Transform menuItemsDir)
        {
            ButtonComponents.Clear();
            PieMenuItems.Clear();

            for (int i = 0; i < menuItemsDir.childCount; i++)
            {
                Transform menuItemTransform = menuItemsDir.GetChild(i);
                Button button = menuItemTransform.GetComponent<Button>();
                PieMenuItem pieMenuItem = menuItemTransform.GetComponent<PieMenuItem>();

                if (button != null && pieMenuItem != null)
                {
                    ButtonComponents.Add(i, button);
                    PieMenuItems.Add(i, pieMenuItem);
                }
            }
        }

        public PieMenuItem GetMenuItem(int id)
        {
            PieMenuItems.TryGetValue(id, out PieMenuItem menuItem);
            return menuItem;
        }

        public void RemoveMenuItem(int id)
        {
            if (PieMenuItems.ContainsKey(id))
            {
                Destroy(PieMenuItems[id].gameObject);
                PieMenuItems.Remove(id);
                ButtonComponents.Remove(id);
            }
        }
    }
}