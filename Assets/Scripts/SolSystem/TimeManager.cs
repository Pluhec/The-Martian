using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Časový cyklus")]
    public DateTime currentTime;
    private DateTime startTime = new DateTime(1, 1, 1, 8, 0, 0);
    private DateTime endTime = new DateTime(1, 1, 1, 22, 0, 0);

    [Header("Osvětlení")]
    // Přetáhněte sem referenci na Light objekt (např. Directional Light)
    public Light sunLight;

    private void Start()
    {
        ResetTime();
    }

    // Resetuje čas na počáteční hodnotu
    public void ResetTime()
    {
        currentTime = startTime;
        AdjustLighting();
    }

    // Aktualizuje herní čas na základě procentuálního dokončení questů (0 až 1)
    public void UpdateTime(float questCompletionProgress)
    {
        // Vypočítáme celkovou dobu dne
        TimeSpan totalDuration = endTime - startTime;
        // Vypočítáme nový čas
        double ticks = totalDuration.Ticks * Mathf.Clamp01(questCompletionProgress);
        TimeSpan offset = new TimeSpan((long)ticks);
        currentTime = startTime + offset;
        Debug.Log("TimeManager: Aktuální čas " + currentTime.ToShortTimeString());
        AdjustLighting();
    }

    // Upraví barvu světla podle aktuálního času
    public void AdjustLighting()
    {
        // Jednoduché rozdělení: 
        // do 12:00 – ranní (červené světlo), 12:00 až 18:00 – polední (bílé světlo), 18:00 až 22:00 – večerní/nocní (modré světlo)
        int hour = currentTime.Hour;
        if (hour < 12)
        {
            sunLight.color = Color.red;
        }
        else if (hour < 18)
        {
            sunLight.color = Color.white;
        }
        else
        {
            sunLight.color = Color.blue;
        }
    }
}