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
    public Image engineBarFill;

    [Header("Gear Buttons")] 
    public Button btnP;
    public Button btnR;
    public Button btnN;
    public Button btnD;

    private Rover rover;

    private void Awake()
    {
        rover = FindObjectOfType<Rover>();
    }

    private void OnEnable()
    {
        // registrace listener na UI až se spusti canvas
        startStopBtn.onClick.AddListener(OnStartStopClicked);
        btnP.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.P));
        btnR.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.R));
        btnN.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.N));
        btnD.onClick.AddListener(() => DoGear(TransmissionSystem.Gear.D));
    }

    private void OnDisable()
    {
        startStopBtn.onClick.RemoveAllListeners();
        btnP.onClick.RemoveAllListeners();
        btnR.onClick.RemoveAllListeners();
        btnN.onClick.RemoveAllListeners();
        btnD.onClick.RemoveAllListeners();
    }
    
    private void OnStartStopClicked()
    {
        if (rover.transmission.currentGear != TransmissionSystem.Gear.P)
        {
            Debug.Log("Nelze vypnout motor, není zařazený parking!");
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

    private void Update()
    {
        if (!rover.IsDriverInside)
            return;

        // predchystane na UI baterky
        var battNorm = rover.battery.currentEnergy / rover.battery.maxEnergy;
        var battOn = Mathf.Clamp(Mathf.RoundToInt(battNorm * batterySegments.Length), 0, batterySegments.Length);
        for (var i = 0; i < batterySegments.Length; i++)
            batterySegments[i].enabled = i < battOn;

        // predchystane na UI boost
        var boostNorm = rover.boost.CooldownRatio;
        var boostOn = Mathf.Clamp(Mathf.RoundToInt((1f - boostNorm) * boostSegments.Length), 0, boostSegments.Length);
        for (var i = 0; i < boostSegments.Length; i++)
            boostSegments[i].enabled = i < boostOn;

        // barvy starstop indikatoru
        var on = rover.transmission.isEngineOn;
        engineBarFill.fillAmount = on ? 1f : 0f;
        engineBarFill.color = on ? Color.green : Color.gray;

        // parking jde zaradit i bez behu motoru, ostatni ne
        btnP.interactable = rover.transmission.currentGear != TransmissionSystem.Gear.P;
        btnR.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.R;
        btnN.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.N;
        btnD.interactable = on && rover.transmission.currentGear != TransmissionSystem.Gear.D;
    }
}