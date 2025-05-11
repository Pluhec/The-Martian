using System.Collections.Generic;
using UnityEngine;

public class Rover : InteractableObject
{
    [Header("UI & Seat")] 
    public Transform seat;
    public GameObject roverUICanvas;

    [Header("Subsystems")] 
    public BatterySystem battery;
    public BoostSystem boost;
    public TransmissionSystem transmission;

    [Header("Engine Consumption")]
    public float engineConsumptionRate = 0.02f; 

    private bool driverInside = false;
    public bool IsDriverInside => driverInside;

    private GameObject playerGO;
    private Movement playerMovement;
    private PlayerInteraction2D playerInteract;

    private void Start()
    {
        if (roverUICanvas != null)
            roverUICanvas.SetActive(false);
    }

    private void Awake()
    {
        actions.Add("Enter Rover");
        actions.Add("Exit Rover");
        // actions.Add("Load Cargo"); // potom dodelat to nakladani atd..
    }

    public override void PerformAction(string action)
    {
        Debug.Log($"[Rover] PerformAction: '{action}', inside={driverInside}");
        if (action == "Enter Rover" && !driverInside)
            EnterRover();
        else if (action == "Exit Rover" && driverInside)
        {
            if (transmission.currentGear != TransmissionSystem.Gear.P)
            {
                Debug.Log("Nelze vystoupit, není zařazený parking! notification");
                return;
            }
            ExitRover();            
        }
    }

    private void EnterRover()
    {
        Debug.Log("[Rover] EnterRover()");
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerGO.GetComponent<Movement>();
        playerInteract = playerGO.GetComponent<PlayerInteraction2D>();

        // vypnuti jen pohybu a ne celeho hrace - avg fix na 20 minut
        playerMovement.enabled = false;

        playerGO.transform.SetParent(seat, false);
        playerGO.transform.localPosition = Vector3.zero;

        roverUICanvas.SetActive(true);
        driverInside = true;
    }

    private void ExitRover()
    {
        Debug.Log("[Rover] ExitRover()");
        if (transmission.isEngineOn)
            transmission.ToggleEngine();
        transmission.SetGear(TransmissionSystem.Gear.P);

        playerGO.transform.SetParent(null);
        playerMovement.enabled = true;

        roverUICanvas.SetActive(false);
        driverInside = false;
    }

    private void Update()
    {
        if (!driverInside)
            return;

        // motor papa energii i kdyz je nastartovany
        if (transmission.isEngineOn)
        {
            float consume = engineConsumptionRate * Time.deltaTime;
            if (boost.IsBoosting)
                consume *= boost.consumptionMultiplier;

            bool hasPower = battery.Consume(consume);
            if (!hasPower)
            {
                Debug.Log("[Rover] Baterie vybitá, motor vypínám");
                transmission.ToggleEngine();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("[Rover] Shift stisknut – pokus o boost");
            boost.TryBoost();
        }
    }
}