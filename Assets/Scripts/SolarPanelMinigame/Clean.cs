using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Clean : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Texture2D dirtMask;    
    [SerializeField] private Texture2D brush;       
    [SerializeField] private Material dirtMaterial; 
    
    [Header("Brush Settings")]
    [SerializeField, Range(0.1f, 5f)] 
    private float brushSize = 1f;      
    [SerializeField, Range(0.01f, 1f)] 
    private float cleanSpeed = 0.3f;   

    private Texture2D activeMask;      
    private SpriteRenderer renderer;
    private float initialDirtAmount;
    private float currentDirtAmount;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        InitializeDirtMask();
        CalculateInitialDirtAmount();
    }
    
    private void InitializeDirtMask()
    {
        // vytvoreni nove masky
        activeMask = new Texture2D(
            dirtMask.width, 
            dirtMask.height, 
            TextureFormat.RGBA32, 
            false
        ) {
            wrapMode = TextureWrapMode.Clamp,  // zabraneni opakovani se textury
            filterMode = FilterMode.Bilinear   // vyhlazeni hran textury
        };
        
        activeMask.SetPixels(dirtMask.GetPixels());
        activeMask.Apply();
        
        dirtMaterial.SetTexture("_DirtMask", activeMask);
    }
    
    private void CalculateInitialDirtAmount()
    {
        initialDirtAmount = 0;
        currentDirtAmount = 0;
        
        for (int x = 0; x < activeMask.width; x++)
        {
            for (int y = 0; y < activeMask.height; y++)
            {
                initialDirtAmount += activeMask.GetPixel(x, y).g;
            }
        }
        currentDirtAmount = initialDirtAmount;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CleanAtMousePosition();
        }

        Debug.Log(GetRemainingDirtPercentage());
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
            (localPos.x / renderer.sprite.bounds.size.x) + 0.5f,
            (localPos.y / renderer.sprite.bounds.size.y) + 0.5f
        );
        
        // zmena na pixelove souradnice
        return new Vector2(
            Mathf.RoundToInt(normalized.x * activeMask.width),
            Mathf.RoundToInt(normalized.y * activeMask.height)
        );
    }
    
    private void ApplyBrush(Vector2 centerPixel)
    {
        // velikost stetce
        int width = Mathf.RoundToInt(brush.width * brushSize);
        int height = Mathf.RoundToInt(brush.height * brushSize);
        
        int startX = Mathf.Clamp((int)centerPixel.x - width/2, 0, activeMask.width-1);
        int startY = Mathf.Clamp((int)centerPixel.y - height/2, 0, activeMask.height-1);
        int endX = Mathf.Clamp((int)centerPixel.x + width/2, 0, activeMask.width-1);
        int endY = Mathf.Clamp((int)centerPixel.y + height/2, 0, activeMask.height-1);

        float dirtRemoved = 0f;

        // loop pro vsechny pixely v masce
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                float u = (x - startX) / (float)width;
                float v = (y - startY) / (float)height;
                
                float brushValue = brush.GetPixelBilinear(u, v).g;
                float cleanAmount = brushValue * cleanSpeed * Time.deltaTime;
                
                Color currentDirt = activeMask.GetPixel(x, y);
                float newValue = Mathf.Clamp01(currentDirt.g - cleanAmount);
                
                dirtRemoved += currentDirt.g - newValue;
                activeMask.SetPixel(x, y, new Color(0, newValue, 0));
            }
        }
        
        activeMask.Apply();
        currentDirtAmount -= dirtRemoved;
    }

    public float GetCleanPercentage()
    {
        if (initialDirtAmount <= 0) return 1f;
        return 1f - (currentDirtAmount / initialDirtAmount);
    }

    public float GetRemainingDirtPercentage()
    {
        return 1f - GetCleanPercentage();
    }
}