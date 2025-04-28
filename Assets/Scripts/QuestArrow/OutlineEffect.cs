using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineEffect : MonoBehaviour
{
    [Tooltip("Barva Outliny")]    
    public Color outlineColor = new Color(0f, 254f / 255f, 254f / 255f, 1f);
    [Tooltip("Velikost Outliny")]    
    public Vector3 outlineScale = new Vector3(1.1f, 1.1f, 1f);

    private GameObject outlineObject;
    private SpriteRenderer outlineSpriteRenderer;

    void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        
        outlineObject = new GameObject(name + "_Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = outlineScale;
        
        outlineSpriteRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        outlineSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        outlineSpriteRenderer.color = outlineColor;
        outlineObject.SetActive(false);
    }
    
    public void SetOutlineEnabled(bool enabled)
    {
        if (outlineObject != null)
            outlineObject.SetActive(enabled);
    }
}