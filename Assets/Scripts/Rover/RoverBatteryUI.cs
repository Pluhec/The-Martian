using UnityEngine;
using UnityEngine.UI;

public class RoverBatteryUI : MonoBehaviour
{
    [Header("Battery UI")]
    public Image[] batterySegments;

    [Header("Boost UI")]
    public Image[] boostSegments;
    
    private Rover rover;

    private void Awake()
    {
        rover = GetComponent<Rover>();
        if (rover == null)
            Debug.LogError("RoverBatteryUI: nenalezen Rover komponent!");
    }

    private void Update()
    {
        if (!rover.IsDriverInside) return;

        UpdateBatteryUI();
        UpdateBoostUI();
    }

    private void UpdateBatteryUI()
    {
        float batteryRatio = rover.battery.currentEnergy / rover.battery.maxEnergy;
        int activeSegments = Mathf.Clamp(Mathf.RoundToInt(batteryRatio * batterySegments.Length), 0, batterySegments.Length);

        for (int i = 0; i < batterySegments.Length; i++)
            batterySegments[i].enabled = i < activeSegments;
    }

    private void UpdateBoostUI()
    {
        float boostRatio = rover.boost.CooldownRatio;
        int activeBoostSegments = Mathf.Clamp(Mathf.RoundToInt((1f - boostRatio) * boostSegments.Length), 0, boostSegments.Length);

        for (int i = 0; i < boostSegments.Length; i++)
            boostSegments[i].enabled = i < activeBoostSegments;
    }
}