using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

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

    [Header("Camera Zoom")]
    public CinemachineCamera vcam;
    public float outsideOrthoSize = 5f;
    public float insideOrthoSize = 8f;
    public float zoomDuration = 0.75f; 
    private float originalOrthoSize;
    private Coroutine cameraZoomCoroutine;

    private bool driverInside = false;
    public bool IsDriverInside => driverInside;

    private GameObject playerGO;
    private Movement playerMovement;
    private PlayerInteraction2D playerInteract;

    private void Start()
    {
        if (roverUICanvas != null)
            roverUICanvas.SetActive(false);

        if (vcam != null)
            originalOrthoSize = vcam.Lens.OrthographicSize;
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

        // cam scale
        if (vcam != null)
        {
            vcam.Follow = transform;
            if (cameraZoomCoroutine != null)
                StopCoroutine(cameraZoomCoroutine);
            cameraZoomCoroutine = StartCoroutine(SmoothCameraZoom(vcam.Lens.OrthographicSize, insideOrthoSize));
        }
    }

    private void ExitRover()
    {
        Debug.Log("[Rover] ExitRover()");
        if (transmission.isEngineOn)
            transmission.ToggleEngine();
        transmission.SetGear(TransmissionSystem.Gear.P);
        
        playerGO.transform.SetParent(null);
        
        Vector3 exitPosition = transform.position - transform.right * 0.8f;
        playerGO.transform.position = exitPosition;
        
        playerGO.transform.rotation = Quaternion.identity;

        playerMovement.enabled = true;

        roverUICanvas.SetActive(false);
        driverInside = false;
        
        if (vcam != null)
        {
            vcam.Follow = playerGO.transform;
            if (cameraZoomCoroutine != null)
                StopCoroutine(cameraZoomCoroutine);
            cameraZoomCoroutine = StartCoroutine(SmoothCameraZoom(vcam.Lens.OrthographicSize, originalOrthoSize));
        }
    }

    // Turbovypocet pro zoom a unzoom, ale vypada to mega toptier
    private IEnumerator SmoothCameraZoom(float startSize, float targetSize)
    {
        float elapsedTime = 0;
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / zoomDuration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            float newSize = Mathf.Lerp(startSize, targetSize, smoothT);
            vcam.Lens.OrthographicSize = newSize;
            yield return null;
        }
        vcam.Lens.OrthographicSize = targetSize;
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