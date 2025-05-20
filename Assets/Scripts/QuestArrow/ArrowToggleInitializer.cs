using UnityEngine;
using UnityEngine.UI;

public class ArrowToggleInitializer : MonoBehaviour
{
    [SerializeField] private Toggle arrowToggle;

    private void Awake()
    {
        arrowToggle.isOn = GameManager.Instance.arrowEnabled;
        
        arrowToggle.onValueChanged.AddListener(GameManager.Instance.SetArrowEnabled);
    }
}