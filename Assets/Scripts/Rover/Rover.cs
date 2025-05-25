using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class Rover : InteractableObject
{
    [Header("UI & Seat")] 
    public Transform seat;
    public GameObject roverUICanvas;
    public GameObject playerUI;         
    private GameObject inventoryUI => Inventory.Instance.gameObject;

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
        actions.Add("Hide Or Show Cargo Zone");
        actions.Add("Load Or Unload Cargo");
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
        else if (action == "Hide Or Show Cargo Zone")
        {
            showCargoZone = !showCargoZone;
            outline.ToggleOutline(showCargoZone);
        }
        else if (action == "Load Or Unload Cargo")
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
        var def = item.GetComponent<ItemDefinition>();
        if (def == null)
        {
            Debug.LogWarning($"[Rover] LoadableItem '{item.name}' nemá ItemDefinition!");
            return;
        }

        item.originalPrefab = PrefabRegistry.Instance.Get(def.itemID);
        if (item.originalPrefab == null)
            Debug.LogWarning($"[Rover] Prefab s ID '{def.itemID}' nenalezen v PrefabRegistry.");

        item.transform.SetParent(cargo, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one * 0.5f;
        item.isLoaded = true;
        outline.ToggleOutline(false);
        var col = item.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        currentCargo = item;
        Debug.Log($"[Rover] naložil jsem: {item.name}");
    }


    private void UnloadCargo()
    {
        if (currentCargo == null) return;

        currentCargo.transform.localScale = Vector3.one;
        currentCargo.transform.SetParent(null, true);
        currentCargo.transform.position = cargoZone.position;
        currentCargo.transform.rotation = Quaternion.identity;

        var prefab = currentCargo.originalPrefab;
        if (prefab != null)
            DroppedItemManager.Instance.AddDroppedItem(prefab, cargoZone.position);
        else
            Debug.LogWarning($"[Rover] Prefab pro '{currentCargo.name}' není nastavený.");

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

        playerUI.SetActive(false);
        inventoryUI.SetActive(false);

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

        playerMovement.Oxygen = playerMovement.MaxOxygen;
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