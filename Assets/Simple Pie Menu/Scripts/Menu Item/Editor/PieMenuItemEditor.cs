using UnityEditor;
using UnityEngine;
using UnityEngine.UI; // Add this line

namespace SimplePieMenu
{
    [CustomEditor(typeof(PieMenuItem))]
    public class PieMenuItemEditor : Editor
    {
        private PieMenuItem pieMenuItem;
        private SerializedProperty header;
        private SerializedProperty details;

        private void OnEnable()
        {
            pieMenuItem = (PieMenuItem)target;
            header = serializedObject.FindProperty("header");
            details = serializedObject.FindProperty("details");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(header);
            EditorGUILayout.PropertyField(details);

            if (GUILayout.Button("Change Color"))
            {
                ChangeColor();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ChangeColor()
        {
            Button button = GetButton();
            if (button != null)
            {
                button.image.color = Color.red;
            }
        }

        private Button GetButton()
        {
            if (pieMenuItem == null)
            {
                Debug.LogError("PieMenuItem is not assigned.");
                return null;
            }

            Button button = pieMenuItem.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("Button component is not found.");
            }

            return button;
        }
    }
}