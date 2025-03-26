using UnityEngine;

public class Clean : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Texture2D _dirtMaskBase;
    [SerializeField] private Texture2D _brush;
    [SerializeField] private Material _material;

    private Texture2D _templateDirtMask;
    private Vector2Int _lastPixelPosition;

    private void Start()
    {
        CreateTexture();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                CleanAtPosition(hit.point, hit.collider);
            }
        }
    }

    private void CleanAtPosition(Vector2 worldPosition, Collider2D collider)
    {
        SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        // prevod na lokalni souradnice
        Vector2 localPosition = collider.transform.InverseTransformPoint(worldPosition);
        Sprite sprite = spriteRenderer.sprite;
        
        Rect spriteRect = sprite.rect;
        Vector2 textureCoord = new Vector2(
            (localPosition.x + sprite.bounds.extents.x) / sprite.bounds.size.x,
            (localPosition.y + sprite.bounds.extents.y) / sprite.bounds.size.y
        );

        // prevod na pixelove souradnice
        int pixelX = (int)(textureCoord.x * _templateDirtMask.width);
        int pixelY = (int)(textureCoord.y * _templateDirtMask.height);
        
        ApplyBrush(pixelX, pixelY);
    }

    private void ApplyBrush(int centerX, int centerY)
    {
        int startX = Mathf.Clamp(centerX - _brush.width / 2, 0, _templateDirtMask.width - 1);
        int startY = Mathf.Clamp(centerY - _brush.height / 2, 0, _templateDirtMask.height - 1);
        int endX = Mathf.Clamp(centerX + _brush.width / 2, 0, _templateDirtMask.width - 1);
        int endY = Mathf.Clamp(centerY + _brush.height / 2, 0, _templateDirtMask.height - 1);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                int brushX = x - (centerX - _brush.width / 2);
                int brushY = y - (centerY - _brush.height / 2);

                if (brushX >= 0 && brushX < _brush.width && brushY >= 0 && brushY < _brush.height)
                {
                    Color brushPixel = _brush.GetPixel(brushX, brushY);
                    Color maskPixel = _templateDirtMask.GetPixel(x, y);
                    
                    float newValue = Mathf.Clamp01(maskPixel.g - brushPixel.g * Time.deltaTime);
                    _templateDirtMask.SetPixel(x, y, new Color(0, newValue, 0));
                }
            }
        }

        _templateDirtMask.Apply();
        _material.SetTexture("_DirtMask", _templateDirtMask);
    }

    private void CreateTexture()
    {
        if (_dirtMaskBase == null)
        {
            Debug.LogError("Dirt Mask Base texture is not assigned!");
            return;
        }

        _templateDirtMask = new Texture2D(_dirtMaskBase.width, _dirtMaskBase.height, TextureFormat.RGBA32, false);
        _templateDirtMask.SetPixels(_dirtMaskBase.GetPixels());
        _templateDirtMask.Apply();

        _material.SetTexture("_DirtMask", _templateDirtMask);
    }
}