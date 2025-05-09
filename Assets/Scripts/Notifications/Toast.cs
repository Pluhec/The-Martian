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
    public float slideDuration = 0.5f;
    public float displayDuration = 7f;
    public float hiddenY = -200f;
    public float visibleY = 50f;

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
        // nahoru
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float y = Mathf.Lerp(hiddenY, visibleY, t / slideDuration);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        yield return new WaitForSeconds(displayDuration);

        // dolu
        t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float y = Mathf.Lerp(visibleY, hiddenY, t / slideDuration);
            panelRect.anchoredPosition = new Vector2(0, y);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}