using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Clean : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Texture2D dirtMask;    
    [SerializeField] private Texture2D brush;       
    [SerializeField] private Material dirtMaterial; 
    [SerializeField] private CleanedUI cleanedUI;
    
    [Header("Brush Settings")]
    [SerializeField, Range(0.1f, 5f)] 
    private float brushSize = 1f;      
    [SerializeField, Range(0.01f, 1f)] 
    private float cleanSpeed = 0.3f;
    
    private AudioManager audioManager;
    private SpriteRenderer renderer;

    private Texture2D activeMask;      
    private float initialDirtAmount;
    private float currentDirtAmount;
    
    private bool isCleaning = false;
    private bool dirtInitialized = false;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    private void InitializeDirtMaskInternal()
    {
        activeMask = new Texture2D(
            dirtMask.width, 
            dirtMask.height, 
            TextureFormat.RGBA32, 
            false
        ){
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        
        activeMask.SetPixels(dirtMask.GetPixels());
        activeMask.Apply();
        dirtMaterial.SetTexture("_DirtMask", activeMask);
    }

    private void CalculateInitialDirtAmount()
    {
        initialDirtAmount = 0f;
        for (int x = 0; x < activeMask.width; x++)
            for (int y = 0; y < activeMask.height; y++)
                initialDirtAmount += activeMask.GetPixel(x, y).g;

        currentDirtAmount = initialDirtAmount;
    }
    
    public void ResetToDirty()
    {
        if (dirtInitialized) 
            return;

        InitializeDirtMaskInternal();
        CalculateInitialDirtAmount();
        cleanedUI.UpdateBar(0f);
        cleanedUI.UpdateText(0);
        cleanedUI.ResetSoundFlag();

        dirtInitialized = true;
    }

    /// <summary>
    /// Volat po dokončení questu nebo při opakovaném otevírání,
    /// aby zůstal panel trvale čistý.
    /// </summary>
    public void ClearDirtPermanently()
    {
        if (dirtInitialized && currentDirtAmount <= 0f)
            return;

        int w = dirtMask.width, h = dirtMask.height;
        activeMask = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color[] clear = new Color[w * h];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = new Color(0, 0, 0, 0);

        activeMask.SetPixels(clear);
        activeMask.Apply();
        dirtMaterial.SetTexture("_DirtMask", activeMask);

        initialDirtAmount = currentDirtAmount = 0f;
        cleanedUI.UpdateBar(1f);
        cleanedUI.UpdateText(100);

        dirtInitialized = true;
    }

    private void Update()
    {
        float cleanedPercentage = GetCleanPercentage();
        cleanedUI.UpdateText((int)Math.Round(cleanedPercentage));
        cleanedUI.UpdateBar(cleanedPercentage / 100f);
        
        if (Input.GetMouseButton(0))
        {
            if (!isCleaning)
            {
                isCleaning = true;
                audioManager.PlayCleaningSolarPanel(audioManager.AirCleaningSound);
            }
            CleanAtMousePosition();
        }
        else if (isCleaning)
        {
            isCleaning = false;
            audioManager.StopCleaningSolarPanel();
        }
    }

    private void CleanAtMousePosition()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
            ApplyBrush(WorldToTextureCoords(mouseWorldPos));
    }

    private Vector2 WorldToTextureCoords(Vector2 worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        Vector2 normalized = new Vector2(
            (localPos.x / renderer.sprite.bounds.size.x) + 0.5f,
            (localPos.y / renderer.sprite.bounds.size.y) + 0.5f
        );
        return new Vector2(
            Mathf.RoundToInt(normalized.x * activeMask.width),
            Mathf.RoundToInt(normalized.y * activeMask.height)
        );
    }

    private void ApplyBrush(Vector2 centerPixel)
    {
        int width = Mathf.RoundToInt(brush.width * brushSize);
        int height = Mathf.RoundToInt(brush.height * brushSize);
        
        int startX = Mathf.Clamp((int)centerPixel.x - width/2, 0, activeMask.width-1);
        int startY = Mathf.Clamp((int)centerPixel.y - height/2, 0, activeMask.height-1);
        int endX   = Mathf.Clamp((int)centerPixel.x + width/2, 0, activeMask.width-1);
        int endY   = Mathf.Clamp((int)centerPixel.y + height/2, 0, activeMask.height-1);

        float dirtRemoved = 0f;

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
        if (initialDirtAmount <= 0f) return 100f;
        float remaining = currentDirtAmount / initialDirtAmount;
        if (remaining <= 0.0314f) return 100f;
        return (1f - remaining) * 100f;
    }
}