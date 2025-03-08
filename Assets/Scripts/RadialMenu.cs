using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class radialMenu : MonoBehaviour {
    [Tooltip("Prefab segmentu (UI Button)")]
    public GameObject segmentPrefab;
    public Transform slicesParent; // ✅ Přidáváme SlicesParent

    public bool IsVisible { get; private set; } = false;
    private List<GameObject> segments = new List<GameObject>();

    // Metoda pro zobrazení radial menu s danými akcemi
    public void Show(List<InteractionAction> actions) {
        // Nejprve odstraníme staré segmenty
        foreach (Transform child in slicesParent) {
            Destroy(child.gameObject);
        }
        segments.Clear();

        float angleStep = 360f / actions.Count;
        float currentAngle = -90f; // Počáteční úhel (začínáme nahoře)
        float radius = 120f; // Poloměr kruhu

        foreach (var action in actions) {
            GameObject segment = Instantiate(segmentPrefab, slicesParent); // ✅ Přidáváme segmenty do SlicesParent

            // Umístění segmentu v kruhu
            float posX = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float posY = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            segment.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

            // Rotace segmentu
            segment.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            // Oprava detekce kliknutí
            Button button = segment.GetComponent<Button>();
            if (button == null) {
                Debug.LogError("Segment prefab neobsahuje komponentu Button!");
                continue;
            }

            button.onClick.RemoveAllListeners(); // Zabráníme duplikaci eventů
            button.onClick.AddListener(() => {
                Debug.Log($"Kliknuto na tlačítko: {action.actionName}"); // Debug
                action.actionCallback?.Invoke();
                Hide(); // Zavře menu
            });

            Text textComponent = segment.GetComponentInChildren<Text>();
            if (textComponent != null) {
                textComponent.text = action.actionName;
            }

            Image imageComponent = segment.GetComponentInChildren<Image>();
            if (imageComponent != null && action.actionIcon != null) {
                imageComponent.sprite = action.actionIcon;
            }

            segments.Add(segment);
            currentAngle += angleStep; // Posun na další výseč
        }

        gameObject.SetActive(true);
        IsVisible = true;
        StartCoroutine(FadeIn()); // Animace zobrazení
    }

    // Metoda pro skrytí menu (Zavře se pouze po kliknutí na tlačítko)
    public void Hide() {
        Debug.Log("Zavírám Radial Menu...");
        foreach (Transform child in slicesParent) {
            Destroy(child.gameObject);
        }
        segments.Clear();
        gameObject.SetActive(false); // ✅ Skryje celé menu
        IsVisible = false;
    }

    IEnumerator FadeIn() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        float alpha = 0f;
        while (alpha < 1f) {
            alpha += Time.deltaTime * 3;
            canvasGroup.alpha = alpha;
            yield return null;
        }
    }
}
