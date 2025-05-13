using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class Rover : InteractableObject
{
    [Header("UI & Seat")] 
    public Transform seat;
    public GameObject roverUICanvas;
    public GameObject playerUI;         
    public GameObject inventoryUI; 

    [Header("Subsystems")] 
    public BatterySystem battery;
    public BoostSystem boost;
    public TransmissionSystem transmission;

    [Header("Cargo Transforms")] 
    public Transform cargoZone; 
    public Transform cargo; 

    [Header("Engine Consumption")] 
    public float engineConsumptionRate = 0.02f;

    [Header("Camera Zoom")] 
    public CinemachineCamera vcam;
    public float outsideOrthoSize = 5f;
    public float insideOrthoSize = 8f;
    public float zoomDuration = 0.75f;
    private float originalOrthoSize;
    private Coroutine cameraZoomCoroutine;

    [Header("Cargo Zone Settings")] 
    public float loadRadius = 1.5f; 
    private bool showCargoZone = false; 
    private LoadableItem currentCargo = null;
    private CargoZoneOutline outline;
    
    [Header("Audio")]
    public RoverAudioController audioController;

    private bool driverInside = false;
    public bool IsDriverInside => driverInside;

    private GameObject playerGO;
    private Movement playerMovement;
    private PlayerInteraction2D playerInteract;

    private void Awake()
    {
        actions.Add("Enter Rover");
        actions.Add("Hide/Show Cargo Zone");
        actions.Add("Load/Unload Cargo");
    }

    private void Start()
    {
        if (roverUICanvas != null)
            roverUICanvas.SetActive(false);

        if (vcam != null)
            originalOrthoSize = vcam.Lens.OrthographicSize;
        
        if (cargoZone != null)
            outline = cargoZone.GetComponentInChildren<CargoZoneOutline>();

        Debug.Log($"[Rover] Start(): cargoZone={cargoZone}, cargo={cargo}, outline={outline}");
    }

    public override void PerformAction(string action)
    {
        Debug.Log($"[Rover] PerformAction: '{action}', inside={driverInside}");

        if (action == "Enter Rover" && !driverInside)
        {
            EnterRover();
        }
        else if (action == "Hide/Show Cargo Zone")
        {
            showCargoZone = !showCargoZone;
            outline.ToggleOutline(showCargoZone);
        }
        else if (action == "Load/Unload Cargo")
        {
            if (currentCargo == null)
                TryLoadCargo();
            else
                UnloadCargo();
        }
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

    private void TryLoadCargo()
    {
        if (cargoZone == null) return;
        var hits = Physics2D.OverlapCircleAll(cargoZone.position, loadRadius);
        foreach (var c2d in hits)
        {
            var item = c2d.GetComponent<LoadableItem>();
            if (item != null && !item.isLoaded)
            {
                LoadCargo(item);
                return;
            }
        }

        Debug.Log("[Rover] zadny item neni v zone! notifications");
    }

    private void LoadCargo(LoadableItem item)
    {
        // parentuje na cargo 
        item.transform.SetParent(cargo, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one * 0.5f;
        item.isLoaded = true;
        outline.ToggleOutline(false);
        var col = item.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        currentCargo = item;
        Debug.Log($"[Rover] nalozil jsem: {item.name}");
    }

    private void UnloadCargo()
    {
        if (currentCargo == null) return;
        // odparentujeme
        currentCargo.transform.SetParent(null, true);
        // vylozeni presne na pozici cargoZone s nulovou rotaci
        currentCargo.transform.position = cargoZone.position;
        currentCargo.transform.rotation = Quaternion.identity;
        currentCargo.isLoaded = false;
        var col = currentCargo.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        currentCargo = null;
    }

    private void EnterRover()
    {
        Debug.Log("[Rover] EnterRover()");
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerGO.GetComponent<Movement>();
        playerInteract = playerGO.GetComponent<PlayerInteraction2D>();

        // vypnuti UI stejně jako v SolarPanel
        playerUI.SetActive(false);
        inventoryUI.SetActive(false);

        // vypnuti jen pohybu a ne celeho hrace
        playerMovement.enabled = false;
        
        playerGO.SetActive(false);
        playerGO.transform.SetParent(seat, false);
        playerGO.transform.localPosition = Vector3.zero;

        roverUICanvas.SetActive(true);
        driverInside = true;

        if (vcam != null)
        {
            vcam.Follow = transform;
            if (cameraZoomCoroutine != null)
                StopCoroutine(cameraZoomCoroutine);
            cameraZoomCoroutine = StartCoroutine(
                SmoothCameraZoom(vcam.Lens.OrthographicSize, insideOrthoSize)
            );
        }
    }

    private void ExitRover()
    {
        Debug.Log("[Rover] ExitRover()");
        if (transmission.isEngineOn)
            transmission.ToggleEngine();
        transmission.SetGear(TransmissionSystem.Gear.P);

        playerGO.transform.SetParent(null);
        var exitPos = transform.position - transform.right * 0.8f;
        playerGO.transform.position = exitPos;
        playerGO.transform.rotation = Quaternion.identity;

        // zapnuti UI stejně jako v SolarPanel
        playerUI.SetActive(true);
        inventoryUI.SetActive(true);

        playerMovement.enabled = true;
        playerGO.SetActive(true);
        roverUICanvas.SetActive(false);
        driverInside = false;

        if (vcam != null)
        {
            vcam.Follow = playerGO.transform;
            if (cameraZoomCoroutine != null)
                StopCoroutine(cameraZoomCoroutine);
            cameraZoomCoroutine = StartCoroutine(
                SmoothCameraZoom(vcam.Lens.OrthographicSize, originalOrthoSize)
            );
        }
    }

    private IEnumerator SmoothCameraZoom(float startSize, float targetSize)
    {
        var elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / zoomDuration);
            var smoothT = Mathf.SmoothStep(0f, 1f, t);
            var newSize = Mathf.Lerp(startSize, targetSize, smoothT);
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
            var consume = engineConsumptionRate * Time.deltaTime;
            if (boost.IsBoosting)
                consume *= boost.consumptionMultiplier;

            var hasPower = battery.Consume(consume);
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