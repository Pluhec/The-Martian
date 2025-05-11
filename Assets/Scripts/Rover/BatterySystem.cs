using UnityEngine;

public class BatterySystem : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    // Ptom se pridaji dalsi metody pro nabijeni, vybijeni atd..
}