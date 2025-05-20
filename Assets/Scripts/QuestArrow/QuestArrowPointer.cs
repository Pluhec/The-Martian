using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class QuestArrowPointer : MonoBehaviour
{
    public RectTransform arrowRectTransform;
    public Canvas canvas;
    public float screenEdgeBuffer = 30f;

    private Camera mainCamera;
    private Transform target;
    private RectTransform canvasRect;
    private Image arrowImage;

    private void Awake()
    {
        if (arrowRectTransform == null)
            arrowRectTransform = GetComponent<RectTransform>();

        arrowImage = arrowRectTransform.GetComponent<Image>();
        if (arrowImage == null)
            Debug.LogError("QuestArrowPointer: chybí Image na šipce!");

        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("QuestArrowPointer: nenašla se hlavní kamera!");

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("QuestArrowPointer: Canvas není přiřazen a nenašel se v rodičích!");

        canvasRect = canvas.GetComponent<RectTransform>();
        arrowImage.enabled = false;
    }

    private void Update()
    {
        // 1) Globální vypínač šipky
        if (!GameManager.Instance.arrowEnabled)
        {
            if (arrowImage.enabled)
                arrowImage.enabled = false;
            return;
        }

        // 2) Nemáme target -> skrytí
        if (target == null)
        {
            arrowImage.enabled = false;
            return;
        }

        // 3) Logika on-screen vs off-screen
        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);
        bool onScreen = viewPos.z > 0 &&
                        viewPos.x > 0 && viewPos.x < 1 &&
                        viewPos.y > 0 && viewPos.y < 1;
        if (onScreen)
        {
            arrowImage.enabled = false;
            return;
        }

        // 4) Zobrazit a umístit šipku na okraj Canvasu
        arrowImage.enabled = true;
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        float x = viewPos.x * canvasRect.sizeDelta.x;
        float y = viewPos.y * canvasRect.sizeDelta.y;
        Vector2 canvasPos = new Vector2(x, y) - canvasRect.sizeDelta * 0.5f;

        Vector2 toCenter = canvasPos.normalized;
        canvasPos = toCenter * (canvasRect.sizeDelta * 0.5f - Vector2.one * screenEdgeBuffer);

        arrowRectTransform.anchoredPosition = canvasPos;
        arrowRectTransform.localEulerAngles = new Vector3(0, 0,
            Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (arrowImage != null)
            arrowImage.enabled = (target != null && GameManager.Instance.arrowEnabled);
    }
    
    public void RefreshVisibility()
    {
        if (!GameManager.Instance.arrowEnabled)
        {
            arrowImage.enabled = false;
            return;
        }

        if (target == null)
        {
            arrowImage.enabled = false;
            return;
        }

        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);
        bool onScreen = viewPos.z > 0 &&
                        viewPos.x > 0 && viewPos.x < 1 &&
                        viewPos.y > 0 && viewPos.y < 1;
        arrowImage.enabled = !onScreen;
    }
}