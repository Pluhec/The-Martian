using System.Collections;
using UnityEngine;

public class CargoZoneOutline : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) ToggleOutline(sr.color.a < 0.5f);
    }

    public void ToggleOutline(bool on)
    {
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        if (on) pulseRoutine = StartCoroutine(Pulse());
        else pulseRoutine = StartCoroutine(FadeOut());
    }

    private IEnumerator Pulse()
    {
        var t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            // puls alpha mezi 0.2 a 0.8
            var alpha = (Mathf.Sin(t * 2f) + 1f) * 0.3f + 0.2f;
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        // rychle do nuly
        while (sr.color.a > 0f)
        {
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, c.a - Time.deltaTime * 2f);
            yield return null;
        }

        var cc = sr.color;
        sr.color = new Color(cc.r, cc.g, cc.b, 0f);
    }
}