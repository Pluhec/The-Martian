using UnityEngine;

namespace DefaultNamespace
{
    public class DisableMouseInTerminal : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked; // Uzamkne myš na místě
            Cursor.visible = false; // Skryje kurzor
        }
    }
}