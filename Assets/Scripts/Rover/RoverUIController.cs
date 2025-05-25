using UnityEngine;
using UnityEngine.UI;

public class RoverUIController : MonoBehaviour
{
    [Header("Canvas Root")] 
    public GameObject roverUICanvas;

    [Header("Battery Segments")] 
    public Image[] batterySegments;

    [Header("Boost Segments")] 
    public Image[] boostSegments;

    [Header("Engine Button")] 
    public Button startStopBtn;
    public Image  engineBarFill;

    [Header("Gear Buttons")] 
    public Button btnP;
    public Button btnR;
    public Button btnN;
    public Button btnD;

    [Header("Exit Button")] 
    public Button exitBtn;

    [Header("Park Asistent")]
    public Button parkAssistBtn;
    
    private Rover rover;

    private void Awake()
    {
        rover = FindObjectOfType<Rover>();
    }

    private void OnEnable()
    {
        startStopBtn.onClick.AddListener(OnStartStopClicked);
        btnP.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.P));
        btnR.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.R));
        btnN.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.N));
        btnD.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.D));
        exitBtn.onClick.AddListener(OnExitClicked);
        parkAssistBtn.onClick.AddListener(OnParkAssistClicked);
    }

    private void OnDisable()
    {
        startStopBtn.onClick.RemoveAllListeners();
        btnP.onClick.RemoveAllListeners();
        btnR.onClick.RemoveAllListeners();
        btnN.onClick.RemoveAllListeners();
        btnD.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        parkAssistBtn.onClick.RemoveAllListeners();
    }

    private void OnParkAssistClicked()
    {
        rover.PerformAction("Hide/Show Cargo Zone");
    }
    
    private void OnStartStopClicked()
    {
        if (rover.transmission.currentGear != TransmissionSystem.Gear.P)
        {
            Debug.Log("Nelze vypnout motor, není zařazený parking! notification");
            return;
        }
        
        Debug.Log("Motor je vypnuty");
        rover.transmission.ToggleEngine();
    }

    private void DoGear(TransmissionSystem.Gear g)
    {
        Debug.Log($"[UI] GearClicked: {g}");
        rover.transmission.SetGear(g);
    }

    private void OnExitClicked()
    {
        Debug.Log("[UI] ExitClicked");
        rover.PerformAction("Exit Rover");
    }

    private void Update()
    {
        if (!rover.IsDriverInside)
        {
            roverUICanvas.SetActive(false);
            return;
        }

        roverUICanvas.SetActive(true);

        var battNorm = rover.battery.currentEnergy / rover.battery.maxEnergy;
        var battOn   = Mathf.Clamp(Mathf.RoundToInt(battNorm * batterySegments.Length), 0, batterySegments.Length);
        for (var i = 0; i < batterySegments.Length; i++)
            batterySegments[i].enabled = i < battOn;

        var boostNorm = rover.boost.CooldownRatio;
        var boostOn   = Mathf.Clamp(Mathf.RoundToInt((1f - boostNorm) * boostSegments.Length), 0, boostSegments.Length);
        for (var i = 0; i < boostSegments.Length; i++)
            boostSegments[i].enabled = i < boostOn;

        var on = rover.transmission.isEngineOn;
        engineBarFill.fillAmount = on ? 1f : 0f;
        engineBarFill.color      = on ? Color.green : Color.gray;

        btnP.interactable = rover.transmission.currentGear != TransmissionSystem.Gear.P;
        btnR.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.R;
        btnN.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.N;
        btnD.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.D;
        
        exitBtn.gameObject.SetActive(true);
        bool canExit = rover.transmission.currentGear == TransmissionSystem.Gear.P;
        exitBtn.interactable = canExit;
        exitBtn.image.color = canExit ? Color.red : Color.gray;
    }
}