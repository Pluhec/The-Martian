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
    public float bounceStrength = 0.15f;
    [Range(0.1f, 0.5f)]
    public float bounceDuration = 0.2f;

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

        panelRect.anchoredPosition = new Vector2(0, hiddenY);
        StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        float mainDuration = slideDuration * (1f - bounceDuration);

        float t = 0f;
        float overshootY = visibleY + (visibleY - hiddenY) * bounceStrength;

        while (t < mainDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / mainDuration);
            float easedTime = EaseOutQuad(normalizedTime);
            float y = Mathf.Lerp(hiddenY, overshootY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        t = 0f;
        float bounceDurationTime = slideDuration * bounceDuration;
        float startY = overshootY;

        while (t < bounceDurationTime)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / bounceDurationTime);
            float easedTime = EaseInOutQuad(normalizedTime);
            float y = Mathf.Lerp(startY, visibleY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        panelRect.anchoredPosition = new Vector2(0, visibleY);

        yield return new WaitForSeconds(displayDuration);

        t = 0f;
        float preFallDuration = slideDuration * 0.2f;
        float preFallY = visibleY + (visibleY - hiddenY) * 0.1f;

        while (t < preFallDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / preFallDuration);
            float easedTime = EaseOutQuad(normalizedTime);
            float y = Mathf.Lerp(visibleY, preFallY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        t = 0f;
        float fallDuration = slideDuration * 0.8f;

        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / fallDuration);
            float easedTime = EaseInCubic(normalizedTime);
            float y = Mathf.Lerp(preFallY, hiddenY, easedTime);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }

        panelRect.anchoredPosition = new Vector2(0, hiddenY);

        Destroy(gameObject);
    }

    private float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }

    private float EaseInQuad(float t)
    {
        return t * t;
    }

    private float EaseInCubic(float t)
    {
        return t * t * t;
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2) / 2f;
    }
}