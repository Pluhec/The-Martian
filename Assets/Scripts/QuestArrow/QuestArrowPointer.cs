using UnityEngine;
using UnityEngine.UI;

public class QuestArrowPointer : MonoBehaviour
{
    public RectTransform arrowRectTransform;
    public Canvas canvas;
    public float screenEdgeBuffer = 30f;

    private Camera mainCamera;
    private Transform target;
    private float canvasScaleFactor;

    void Start()
    {
        if (arrowRectTransform == null)
            arrowRectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        canvasScaleFactor = canvas != null ? canvas.scaleFactor : 1f;
        arrowRectTransform.gameObject.SetActive(false);
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        bool isBehind = screenPos.z < 0;
        if (isBehind) screenPos *= -1f;

        Vector2 clamped = new Vector2(
            Mathf.Clamp(screenPos.x, screenEdgeBuffer, Screen.width - screenEdgeBuffer),
            Mathf.Clamp(screenPos.y, screenEdgeBuffer, Screen.height - screenEdgeBuffer)
        );

        arrowRectTransform.position = clamped / canvasScaleFactor;
        arrowRectTransform.gameObject.SetActive(true);

        Vector2 fromCenter = clamped - new Vector2(Screen.width/2f, Screen.height/2f);
        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        if (isBehind) angle += 180f;
        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        arrowRectTransform.gameObject.SetActive(target != null);
    }
}