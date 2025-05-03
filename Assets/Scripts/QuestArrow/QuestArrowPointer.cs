using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class QuestArrowPointer : MonoBehaviour
{
    public RectTransform arrowRectTransform;   // UI šipka
    public Canvas canvas;                      // rodičovský Canvas
    public float screenEdgeBuffer = 30f;

    private Camera mainCamera;
    private Transform target;
    private RectTransform canvasRect;
    private Image arrowImage;

    private void Awake()
    {
        // 1) Arrow RectTransform
        if (arrowRectTransform == null)
            arrowRectTransform = GetComponent<RectTransform>();

        // 2) Image komponenta
        arrowImage = arrowRectTransform.GetComponent<Image>();
        if (arrowImage == null)
            Debug.LogError("QuestArrowPointer: chybí Image na šipce!");

        // 3) Kamera
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("QuestArrowPointer: nenašla se hlavní kamera!");

        // 4) Canvas
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("QuestArrowPointer: Canvas není přiřazen a nenašel se v rodičích!");

        canvasRect = canvas.GetComponent<RectTransform>();

        // 5) Skrytí na startu
        arrowImage.enabled = false;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);
        bool onScreen = viewPos.z > 0 &&
                        viewPos.x > 0 && viewPos.x < 1 &&
                        viewPos.y > 0 && viewPos.y < 1;
        if (onScreen)
        {
            arrowImage.enabled = false;
            return;
        }

        arrowImage.enabled = true;

        // oříznutí v rozsahu [0,1]
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        // přepočet na canvas-space
        float x = viewPos.x * canvasRect.sizeDelta.x;
        float y = viewPos.y * canvasRect.sizeDelta.y;
        Vector2 canvasPos = new Vector2(x, y) - canvasRect.sizeDelta * 0.5f;

        Vector2 toCenter = canvasPos.normalized;
        canvasPos = toCenter * (canvasRect.sizeDelta * 0.5f - Vector2.one * screenEdgeBuffer);

        arrowRectTransform.anchoredPosition = canvasPos;
        arrowRectTransform.localEulerAngles = new Vector3(0, 0,
            Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg);
    }

    /// <summary>
    /// Nastaví nový světový target pro šipku.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (arrowImage != null)
            arrowImage.enabled = (target != null);
    }
}