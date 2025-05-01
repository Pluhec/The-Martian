using UnityEngine;
using UnityEngine.UI;

public class QuestArrowPointer : MonoBehaviour
{
    public RectTransform arrowRectTransform;
    public Canvas canvas;
    public float screenEdgeBuffer = 30f;

    private Camera mainCamera;
    private Transform target;
    private RectTransform canvasRect;
    private Image arrowImage;

    void Start()
    {
        if (arrowRectTransform == null)
            arrowRectTransform = GetComponent<RectTransform>();
        arrowImage = arrowRectTransform.GetComponent<Image>();
        mainCamera = Camera.main;
        canvasRect = canvas.GetComponent<RectTransform>();
        
        arrowImage.enabled = false;
    }

    void Update()
    {
        if (target == null)
            return;
        
        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);

        // detekce jestli je objekt na obrazovce
        bool onScreen = viewPos.z > 0
                        && viewPos.x > 0 && viewPos.x < 1
                        && viewPos.y > 0 && viewPos.y < 1;
        if (onScreen)
        {
            arrowImage.enabled = false;
            return;
        }
        
        arrowImage.enabled = true;
        
        viewPos.x = Mathf.Clamp(viewPos.x, 0f, 1f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0f, 1f);
        
        float x = viewPos.x * canvasRect.sizeDelta.x;
        float y = viewPos.y * canvasRect.sizeDelta.y;
        Vector2 canvasPos = new Vector2(x, y);
        
        canvasPos -= canvasRect.sizeDelta * 0.5f;
        
        Vector2 toCenter = canvasPos.normalized;
        canvasPos = toCenter * (canvasRect.sizeDelta * 0.5f - Vector2.one * screenEdgeBuffer);
        
        arrowRectTransform.anchoredPosition = canvasPos;
        
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
        arrowRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        arrowImage.enabled = (target != null);
    }
}