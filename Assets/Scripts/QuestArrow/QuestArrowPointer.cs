using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ukazuje šipku na okraji obrazovky směrem k aktuálnímu cíli.
/// </summary>
public class QuestArrowPointer : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform arrowRectTransform;  // Drag & drop UI Image
    public Canvas canvas;                     // Screen Space – Overlay
    [Header("Settings")]
    public float screenEdgeBuffer = 30f;      // pixely od okraje

    private Camera mainCamera;
    private Transform target;
    private RectTransform canvasRect;
    private Image arrowImage;

    void Awake()
    {
        arrowRectTransform = arrowRectTransform ?? GetComponent<RectTransform>();
        arrowImage = arrowRectTransform.GetComponent<Image>();
        mainCamera = Camera.main;
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    void Start()
    {
        arrowImage.enabled = false;
    }

    void Update()
    {
        if (target == null)
        {
            var fbGO = GameObject.FindWithTag("QuestFallback");
            target = fbGO != null ? fbGO.transform : null;
        }
        if (target == null) return;

        Vector3 vp = mainCamera.WorldToViewportPoint(target.position);
        bool onScreen = vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
        if (onScreen)
        {
            arrowImage.enabled = false;
            return;
        }

        arrowImage.enabled = true;
        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        float x = vp.x * canvasRect.sizeDelta.x;
        float y = vp.y * canvasRect.sizeDelta.y;
        Vector2 pos = new Vector2(x, y) - canvasRect.sizeDelta * 0.5f;

        Vector2 dir = pos.normalized;
        Vector2 inside = (canvasRect.sizeDelta * 0.5f) - Vector2.one * screenEdgeBuffer;
        pos = dir * inside;

        arrowRectTransform.anchoredPosition = pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// Nový questový cíl; šipka se přepne okamžitě.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (arrowImage == null)
            arrowImage = arrowRectTransform.GetComponent<Image>();
        arrowImage.enabled = (target != null);
    }
}