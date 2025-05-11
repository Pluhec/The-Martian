using UnityEngine;

public class TransmissionSystem : MonoBehaviour
{
    public enum Gear { P, R, N, D }

    [Header("Transmission Settings")]
    public Gear currentGear = Gear.P;
    public bool isEngineOn = false;

    public void ToggleEngine()
    {
        isEngineOn = !isEngineOn;
        Debug.Log($"Stav Motoru: {(isEngineOn ? "ON" : "OFF")}");
    }

    public void SetGear(Gear g)
    {
        currentGear = g;
        Debug.Log($"Prevodovka: prechod na  {g}");
    }
}