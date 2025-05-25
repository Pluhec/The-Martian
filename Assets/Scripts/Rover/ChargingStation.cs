using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargingStation : InteractableObject
{
    [Header("Charging Station")]
    public float chargeRatePerSecond = 50f;

    [Header("Notifications")]
    public GameObject toastPrefab;
    public Transform notificationsParent;

    private Rover rover;
    private Coroutine chargingCoroutine;

    private void Start()
    {
        rover = FindObjectOfType<Rover>();
        if (rover == null)
            Debug.LogError("ChargingStation: Rover not found in scene!");
        
        if (notificationsParent == null)
            notificationsParent = GameObject.FindGameObjectWithTag("NotificationsParent")?.transform;
    }

    private void Awake()
    {
        actions.Add("Charge");
    }

    public override void PerformAction(string action)
    {
        if (action == "Charge")
            StartChargingCoroutine();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rover detectedRover = other.GetComponent<Rover>();
        if (detectedRover != null && detectedRover == rover)
        {
            Debug.Log("[ChargingStation] Rover detected, starting charging");
            StartChargingCoroutine();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rover detectedRover = other.GetComponent<Rover>();
        if (detectedRover != null && detectedRover == rover)
        {
            StopChargingCoroutine();
        }
    }

    private void StartChargingCoroutine()
    {
        StopChargingCoroutine();

        if (toastPrefab != null && notificationsParent != null)
        {
            var go = Instantiate(toastPrefab, notificationsParent);
            float batteryPercentage = (rover.battery.currentEnergy / rover.battery.maxEnergy) * 100;
            go.GetComponent<Toast>().Show("info", $"Rover charging started ({Mathf.RoundToInt(batteryPercentage)}%)");
        }

        chargingCoroutine = StartCoroutine(QuickCharge());
    }

    private void StopChargingCoroutine()
    {
        if (chargingCoroutine != null)
        {
            StopCoroutine(chargingCoroutine);
            chargingCoroutine = null;
        }
    }

    private IEnumerator QuickCharge()
    {
        while (rover.battery.currentEnergy < rover.battery.maxEnergy)
        {
            rover.battery.Recharge(chargeRatePerSecond * Time.deltaTime);
            yield return null;
        }

        if (toastPrefab != null && notificationsParent != null)
        {
            var go = Instantiate(toastPrefab, notificationsParent);
            go.GetComponent<Toast>().Show("info", "Rover is fully Charged");
        }

        chargingCoroutine = null;
    }
}