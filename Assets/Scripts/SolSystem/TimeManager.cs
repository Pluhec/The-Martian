using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Časový cyklus")]
    // public DateTime currentTime;
    private DateTime startTime = new DateTime(1, 1, 1, 8, 0, 0);
    private DateTime endTime = new DateTime(1, 1, 1, 22, 0, 0);

    [Header("Osvětlení")]
    public Light sunLight;
    public DateTime currentTime;

    private void Start()
    {
        ResetTime();
    }
    
    public void ResetTime()
    {
        currentTime = startTime;
        AdjustLighting();
    }
    
    public void UpdateTime(float questCompletionProgress)
    {
        TimeSpan totalDuration = endTime - startTime;
        double ticks = totalDuration.Ticks * Mathf.Clamp01(questCompletionProgress);
        TimeSpan offset = new TimeSpan((long)ticks);
        currentTime = startTime + offset;
        Debug.Log("TimeManager: Aktuální čas " + currentTime.ToShortTimeString());
        AdjustLighting();
    }
    
    public void AdjustLighting()
    {
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