using UnityEngine;

namespace SimplePieMenu
{
    public class PieMenuInfoPanelSettingsHandler : MonoBehaviour
    {
        public void SetActive(PieMenu pieMenu, bool isActive)
        {
            if (pieMenu == null || pieMenu.gameObject == null)
            {
                Debug.LogError("PieMenu or its GameObject is null.");
                return;
            }

            RectTransform rectTransform = pieMenu.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("RectTransform component is not found.");
                return;
            }

            pieMenu.gameObject.SetActive(isActive);
        }
    }
}