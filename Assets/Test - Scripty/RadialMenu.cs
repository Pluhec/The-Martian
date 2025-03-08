using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RadialMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform menuCenter;
    private List<GameObject> buttons = new List<GameObject>();

    void Start()
    {
        gameObject.SetActive(false); // Menu je na začátku skryté
    }

    public void SetupMenu(InteractableObject interactable)
    {
        if (interactable == null)
        {
            Debug.LogError("RadialMenu: interactable objekt je NULL!");
            return;
        }

        gameObject.SetActive(true); // Aktivujeme menu při zobrazení
        ClearMenu();

        List<string> actions = interactable.GetActions();
        Debug.Log($"RadialMenu: Načítám akce pro {interactable.gameObject.name}, počet akcí: {actions.Count}");

        float angleStep = 360f / actions.Count;
        float currentAngle = 0f;
        float radius = 100f;

        foreach (string action in actions)
        {
            Debug.Log($"RadialMenu: Přidávám tlačítko pro akci {action}");

            GameObject button = Instantiate(buttonPrefab, menuCenter);
            buttons.Add(button);

            // Umístění tlačítka do kruhu
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            button.transform.localPosition = new Vector3(x, y, 0);

            // Nastavení textu tlačítka
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = action;
            }

            // Přidání funkce pro kliknutí
            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() =>
            {
                interactable.PerformAction(action); // Spustíme akci
                CloseMenu(); // Zavřeme menu po kliknutí
            });

            currentAngle += angleStep;
        }
    }

    public void ClearMenu()
    {
        foreach (GameObject btn in buttons)
        {
            Destroy(btn);
        }
        buttons.Clear();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false); // Vypneme menu po kliknutí na tlačítko
    }
}
