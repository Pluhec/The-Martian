using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SpawnPoint
{
    public string   key;
    public Transform location;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Seznam všech spawnu ve scéně")]
    public SpawnPoint[] spawnPoints;

    [Header("Efekt přistání v Habu")]
    public float effectDuration   = 1.3f;
    [Header("Extra prodleva pro úplné dokončení efektu")]
    public float extraEffectDelay = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TeleportPlayer(EntranceData.Instance.lastEntranceKey);
        if (EntranceData.Instance.logAfterLoad)
            StartCoroutine(ReturnEffectCo());
    }

    public void TeleportPlayer(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        var player = GameObject.FindWithTag("Player");
        if (!player)
        {
            Debug.Log($"SpawnManager: Hráč nenalezen pro teleport '{key}'.");
            return;
        }
        var sp = spawnPoints.FirstOrDefault(p => p.key == key);
        if (sp?.location != null)
            player.transform.position = sp.location.position;
        else
            Debug.Log($"SpawnManager: Klíč '{key}' nenašel žádný spawnPoint.");
    }

    IEnumerator ReturnEffectCo()
    {
        Debug.Log("[Teleport] Přistání v Habu (log po loadu)");

        var player = GameObject.FindWithTag("Player");
        var rb = player?.GetComponent<Rigidbody2D>();
        RigidbodyConstraints2D originalConstraints = RigidbodyConstraints2D.None;
        if (rb != null)
        {
            originalConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        var effects = AirlockEffectManager.Instance;
        effects?.ControlSmoke(EntranceData.Instance.lastEntranceKey, effectDuration);
        FindObjectOfType<AudioManager>()?.PlayDecompressionSound();

        yield return new WaitForSeconds(effectDuration + extraEffectDelay);

        if (rb != null) rb.constraints = originalConstraints;
        EntranceData.Instance.logAfterLoad = false;
    }
}
