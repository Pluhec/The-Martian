using UnityEngine;

public class AirlockEffectManager : MonoBehaviour
{
    public static AirlockEffectManager Instance { get; private set; }

    [Header("Main air‑lock")]
    public ParticleSystem[] mainAirlock;

    [Header("Second air‑lock")]
    public ParticleSystem[] secondAirlock;

    [Header("Side air‑lock")]
    public ParticleSystem[] sideAirlock;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* ----------  API  ---------- */

    public void Play(string key)  => Set(key, true);
    public void Stop(string key)  => Set(key, false);

    /* ----------  PRIVATE  ---------- */

    void Set(string key, bool play)
    {
        var arr = key switch
        {
            "MainAirlock"   => mainAirlock,
            "SecondAirlock" => secondAirlock,
            "SideAirlock"   => sideAirlock,
            _ => null
        };

        if (arr == null)
        {
            Debug.LogWarning($"AirlockEffectManager: neznámý entranceKey „{key}“");
            return;
        }

        foreach (var ps in arr)
        {
            if (ps == null) continue;

            // pro jistotu aktivujeme GameObject
            if (play)
            {
                if (!ps.gameObject.activeSelf) ps.gameObject.SetActive(true);
                ps.Play(true);
            }
            else
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                // pokud chceš úplně schovat trysky, odkomentuj:
                // ps.gameObject.SetActive(false);
            }
        }
    }
}