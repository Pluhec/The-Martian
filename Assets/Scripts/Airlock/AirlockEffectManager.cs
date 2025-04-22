using UnityEngine;

public class AirlockEffectManager : MonoBehaviour
{
    public static AirlockEffectManager Instance { get; private set; }

    [Header("Main airlock effects")]
    public ParticleSystem[] mainAirlockPS;

    [Header("Second airlock effects")]
    public ParticleSystem[] secondAirlockPS;

    [Header("Side airlock effects")]
    public ParticleSystem[] sideAirlockPS;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string key)
    {
        ParticleSystem[] arr = GetArrayForKey(key);
        if (arr == null) return;
        foreach (var ps in arr) ps.Play();
    }

    public void Stop(string key)
    {
        ParticleSystem[] arr = GetArrayForKey(key);
        if (arr == null) return;
        foreach (var ps in arr) ps.Stop();
    }

    private ParticleSystem[] GetArrayForKey(string key)
    {
        switch (key)
        {
            case "MainAirlock":   return mainAirlockPS;
            case "SecondAirlock": return secondAirlockPS;
            case "SideAirlock":   return sideAirlockPS;
            default: return null;
        }
    }
}