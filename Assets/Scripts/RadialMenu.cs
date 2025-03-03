using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour {
    [Tooltip("Prefab segmentu (UI Button)")]
    public GameObject segmentPrefab;
    public bool IsVisible { get; private set; } = false;
    private List<GameObject> segments = new List<GameObject>();

    // Metoda pro zobrazení radial menu s danými akcemi
    public void Show(List<InteractionAction> actions) {
        // Nejprve odstraníme případné staré segmenty
        foreach(var seg in segments) {
            Destroy(seg);
        }
        segments.Clear();

        // Vypočítáme úhel mezi segmenty
        float angleStep = 360f / actions.Count;
        float currentAngle = 0f;
        foreach (var action in actions) {
            GameObject segment = Instantiate(segmentPrefab, transform);
            segment.transform.localPosition = Vector3.zero; // Vycentrujeme segment

            // Pozicování segmentu pomocí polárních souřadnic
            float radius = 100f; // Nastav dle potřeby
            float posX = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float posY = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            segment.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

            // Přidáme listener na tlačítko, který zavolá akci
            Button button = segment.GetComponent<Button>();
            button.onClick.AddListener(() => { 
                action.actionCallback?.Invoke();
                Hide();
            });

            // Nastavení textu a ikony – předpokládáme, že prefab má Text a Image jako podobjekty
            Text textComponent = segment.GetComponentInChildren<Text>();
            if(textComponent != null) {
                textComponent.text = action.actionName;
            }
            Image imageComponent = segment.GetComponentInChildren<Image>();
            if(imageComponent != null && action.actionIcon != null) {
                imageComponent.sprite = action.actionIcon;
            }
            
            segments.Add(segment);
            currentAngle += angleStep;
        }
        gameObject.SetActive(true);
        IsVisible = true;
    }

    // Metoda pro skrytí menu
    public void Hide() {
        foreach(var seg in segments) {
            Destroy(seg);
        }
        segments.Clear();
        gameObject.SetActive(false);
        IsVisible = false;
    }
}