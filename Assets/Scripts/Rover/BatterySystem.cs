using UnityEngine;

public class BatterySystem : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float consumptionRatePerSecond = 1f;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    public bool Consume(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Max(currentEnergy, 0f);
        // Debug.Log($"[Battery] Consumed {amount:F2}, left {currentEnergy:F2}");
        return currentEnergy > 0f;
    }
    
    public void Recharge(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        Debug.Log($"[Battery] Recharged {amount:F2}, now {currentEnergy:F2}");
    }
}