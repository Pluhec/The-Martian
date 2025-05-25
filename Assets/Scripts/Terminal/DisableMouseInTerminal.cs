using UnityEngine;

namespace DefaultNamespace
{
    public class DisableMouseInTerminal : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}