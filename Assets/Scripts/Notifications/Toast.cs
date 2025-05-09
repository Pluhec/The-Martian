using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toast : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform panelRect;
    public TMP_Text messageText;
    public TMP_Text typeText;
    public Image backgroundImage;

    [Header("Type Sprites")]
    public Sprite normalSprite;
    public Sprite warningSprite;
    public Sprite alertSprite;

    [Header("Animation Settings")]
    public float slideDuration = 0.7f;
    public float displayDuration = 7f;
    public float hiddenY = -200f;
    public float visibleY = 50f;
    
    [Header("Bounce Settings")]
    [Range(0.05f, 0.3f)]
    public float bounceStrength = 0.15f; // Síla odrazu (v procentech přesahu)
    [Range(0.1f, 0.5f)]
    public float bounceDuration = 0.2f;  // Trvání odrazu (jako část celkové animace)

    public void Show(string type, string message)
    {
        messageText.text = message;
        typeText.text = type.ToUpper();
        
        switch(type.ToLower())
        {
            case "warning":
                backgroundImage.sprite = warningSprite;
                typeText.color = Color.yellow;
                break;
            case "alert":
                backgroundImage.sprite = alertSprite;
                typeText.color = Color.red;
                break;
            default:
                backgroundImage.sprite = normalSprite;
                typeText.color = Color.cyan;
                break;
        }

        // start animace
        panelRect.anchoredPosition = new Vector2(0, hiddenY);
        StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        // Celková délka animace s bounce efektem
        float mainDuration = slideDuration * (1f - bounceDuration);
        
        // Fáze 1: Hlavní pohyb nahoru s přestřelením (overshooting)
        float t = 0f;
        float overshootY = visibleY + (visibleY - hiddenY) * bounceStrength; // Cíl s přesahem
        
        while (t < mainDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / mainDuration);
            
            // Použijeme kvadratickou ease-out funkci pro plynulý nástup
            float easedTime = EaseOutQuad(normalizedTime);
            
            // Lineární interpolace mezi výchozí a přestřelenou pozicí
            float y = Mathf.Lerp(hiddenY, overshootY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        // Fáze 2: Odraz zpět na správnou pozici (bounce efekt)
        t = 0f;
        float bounceDurationTime = slideDuration * bounceDuration;
        float startY = overshootY;
        
        while (t < bounceDurationTime)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / bounceDurationTime);
            
            // Pro odraz použijeme ease-in-out, který vytvoří plynulý pohyb
            float easedTime = EaseInOutQuad(normalizedTime);
            
            float y = Mathf.Lerp(startY, visibleY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        // Ujistíme se, že je přesně na cílové pozici
        panelRect.anchoredPosition = new Vector2(0, visibleY);
        
        // Počkáme, než se toast zobrazí požadovanou dobu
        yield return new WaitForSeconds(displayDuration);

        // Animace mizení s bounce efektem
        // Fáze 1: Malý pohyb vzhůru (před pádem)
        t = 0f;
        float preFallDuration = slideDuration * 0.2f;
        float preFallY = visibleY + (visibleY - hiddenY) * 0.1f; // Menší přesah než při nástupu
        
        while (t < preFallDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / preFallDuration);
            
            // Pro předskok používáme ease-out
            float easedTime = EaseOutQuad(normalizedTime);
            
            float y = Mathf.Lerp(visibleY, preFallY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        // Fáze 2: Pád dolů
        t = 0f;
        float fallDuration = slideDuration * 0.8f;
        
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / fallDuration);
            
            // Pro pád používáme ease-in funkci, která vytvoří efekt zrychlení pádu
            float easedTime = EaseInCubic(normalizedTime);
            
            float y = Mathf.Lerp(preFallY, hiddenY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        // Ujistíme se, že je přesně na konečné pozici
        panelRect.anchoredPosition = new Vector2(0, hiddenY);
        
        Destroy(gameObject);
    }
    
    // Kvadratická ease-out funkce
    private float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }
    
    // Kvadratická ease-in funkce
    private float EaseInQuad(float t)
    {
        return t * t;
    }
    
    // Kubická ease-in funkce (výraznější zrychlení)
    private float EaseInCubic(float t)
    {
        return t * t * t;
    }
    
    // Kvadratická ease-in-out funkce
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2) / 2f;
    }
}