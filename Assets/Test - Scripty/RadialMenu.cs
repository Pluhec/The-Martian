using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RadialMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform menuCenter;
    private List<GameObject> buttons = new List<GameObject>();
    
    public void SetupMenu(InteractableObject interactable)
    {
        if (interactable == null)
        {
            Debug.LogError("RadialMenu: interactable objekt je NULL!");
            return;
        }

        gameObject.SetActive(true); // aktivace menu pri zobrazeni
        ClearMenu();

        List<string> actions = interactable.GetActions();
        Debug.Log($"RadialMenu: Nacitam akce pro {interactable.gameObject.name}, pocet akci: {actions.Count}");

        float angleStep = 360f / actions.Count;
        float currentAngle = 0f;
        float radius = 100f;

        foreach (string action in actions)
        {
            Debug.Log($"RadialMenu: Pridavam tlacitko pro akci {action}");

            GameObject button = Instantiate(buttonPrefab, menuCenter);
            buttons.Add(button);

            // umisteni tlacitka do kruhu
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            button.transform.localPosition = new Vector3(x, y, 0);

            // mastaveni textu tlacitka
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = action;
            }

            // pridani funkce pro kliknuti
            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.AddListener(() =>
            {
                interactable.PerformAction(action);
                CloseMenu();
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
        ClearMenu();
        gameObject.SetActive(false);

        // po zavreni menu restart stavu v PlayerInteraction2D
        PlayerInteraction2D playerInteraction = FindObjectOfType<PlayerInteraction2D>();
        if (playerInteraction != null)
        {
            playerInteraction.HideRadialMenu();
        }
    }
}