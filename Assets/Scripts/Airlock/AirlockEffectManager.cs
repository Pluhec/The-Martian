using System.Collections;
using UnityEngine;

public class AirlockEffectManager : MonoBehaviour
{
    public static AirlockEffectManager Instance { get; private set; }

    [Header("Airlock Smoke Particle Systems")]
    public ParticleSystem[] mainAirlockSmoke;

    public ParticleSystem[] secondAirlockSmoke;
    public ParticleSystem[] sideAirlockSmoke;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ControlSmoke(string target, float duration)
    {
        StartCoroutine(ControlSmokeCoroutine(target, duration));
    }

    // EntraceTrigger sem posila cilovou destinaci a delku celeho effektu
    private IEnumerator ControlSmokeCoroutine(string target, float duration)
    {
        ParticleSystem[] selected;
        switch (target)
        {
            case "MainDoorSpawn":
            case "MainDoor": selected = mainAirlockSmoke; break;
            case "SecondDoorSpawn":
            case "SecondDoor": selected = secondAirlockSmoke; break;
            case "SideDoorSpawn":
            case "SideDoor": selected = sideAirlockSmoke; break;
            default: yield break;
        }

        foreach (var ps in selected)
            if (ps != null)
                ps.Play();

        yield return new WaitForSeconds(duration);

        foreach (var ps in selected)
            if (ps != null)
                ps.Stop();
    }
}