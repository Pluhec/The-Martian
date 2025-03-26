using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Clean : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Texture2D _dirtMask;    
    [SerializeField] private Texture2D _brush;       
    [SerializeField] private Material _dirtMaterial; 
    
    [Header("Brush Settings")]
    [SerializeField, Range(0.1f, 5f)] 
    private float _brushSize = 1f;      
    [SerializeField, Range(0.01f, 1f)] 
    private float _cleanSpeed = 0.3f;   

    private Texture2D _activeMask;      
    private SpriteRenderer _renderer;   

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        InitializeDirtMask();
    }
    
    private void InitializeDirtMask()
    {
        // vytvoreni nove masky
        _activeMask = new Texture2D(
            _dirtMask.width, 
            _dirtMask.height, 
            TextureFormat.RGBA32, 
            false
        ) {
            wrapMode = TextureWrapMode.Clamp,  // zabraneni opakovani se textury
            filterMode = FilterMode.Bilinear   // vyhlazeni hran textury
        };
        
        _activeMask.SetPixels(_dirtMask.GetPixels());
        _activeMask.Apply();
        
        _dirtMaterial.SetTexture("_DirtMask", _activeMask);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CleanAtMousePosition();
        }
    }

    private void CleanAtMousePosition()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        
        // kontrola jestli je mys v panelu
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            ApplyBrush(WorldToTextureCoords(mouseWorldPos));
        }
    }

    private Vector2 WorldToTextureCoords(Vector2 worldPos)
    {
        // prevod na realne souradnice
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        
        // zmena rozsahu na pouze 1 a 0
        Vector2 normalized = new Vector2(
            (localPos.x / _renderer.sprite.bounds.size.x) + 0.5f,
            (localPos.y / _renderer.sprite.bounds.size.y) + 0.5f
        );
        
        // zmena na pixelove souradnice
        return new Vector2(
            Mathf.RoundToInt(normalized.x * _activeMask.width),
            Mathf.RoundToInt(normalized.y * _activeMask.height)
        );
    }
    
    private void ApplyBrush(Vector2 centerPixel)
    {
        // velikost stetce
        int width = Mathf.RoundToInt(_brush.width * _brushSize);
        int height = Mathf.RoundToInt(_brush.height * _brushSize);
        
        int startX = Mathf.Clamp((int)centerPixel.x - width/2, 0, _activeMask.width-1);
        int startY = Mathf.Clamp((int)centerPixel.y - height/2, 0, _activeMask.height-1);
        int endX = Mathf.Clamp((int)centerPixel.x + width/2, 0, _activeMask.width-1);
        int endY = Mathf.Clamp((int)centerPixel.y + height/2, 0, _activeMask.height-1);

        // loop pro vsechny pixely v masce
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                float u = (x - startX) / (float)width;
                float v = (y - startY) / (float)height;
                
                float brushValue = _brush.GetPixelBilinear(u, v).g;
                
                float cleanAmount = brushValue * _cleanSpeed * Time.deltaTime;
                
                Color currentDirt = _activeMask.GetPixel(x, y);
                
                float newValue = Mathf.Clamp01(currentDirt.g - cleanAmount);
                
                _activeMask.SetPixel(x, y, new Color(0, newValue, 0));
            }
        }
        
        _activeMask.Apply();
    }
}