using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargingStation : InteractableObject
{
    [Header("Charging Station")]
    public float chargeRatePerSecond = 50f;

    private Rover rover;

    private void Start()
    {
        rover = FindObjectOfType<Rover>();
        if (rover == null)
            Debug.LogError("ChargingStation: nenalezen Rover ve scéně!");
    }
    
    private void Awake()
    {
        actions.Add("Charge");
    }

    public override void PerformAction(string action)
    {
        if (action == "Charge")
            StartCoroutine(QuickCharge());
    }

    private IEnumerator QuickCharge()
    {
        Debug.Log("[ChargingStation] Začínám dobíjet");
        while (rover.battery.currentEnergy < rover.battery.maxEnergy)
        {
            rover.battery.Recharge(chargeRatePerSecond * Time.deltaTime);
            yield return null;
        }
        Debug.Log("[ChargingStation] Baterie naplněna");
    }
}