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

        // prepinani uvnitr vennku pro footstepManager - audio
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var footstepManager = player.GetComponent<FootstepManager>();
            if (footstepManager != null)
            {
                footstepManager.isOutside = scene.name == "Mars"; 
            }
        }

        // Efekt při návratu do Habu
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
        var player   = GameObject.FindWithTag("Player");
        var movement = player?.GetComponent<Movement>();
        var animator = player?.GetComponent<Animator>();
        
        if (movement != null) 
            movement.enabled = false;
        if (animator != null)
        {
            animator.Play("Idle", 0, 0f);
        }

        // spust kour a zvuk
        var effects = AirlockEffectManager.Instance;
        effects?.ControlSmoke(EntranceData.Instance.lastEntranceKey, effectDuration);
        FindObjectOfType<AudioManager>()?.PlayDecompressionSound();

        // pocka az dojedou efekty
        yield return new WaitForSeconds(effectDuration + extraEffectDelay);
        
        if (movement != null) 
            movement.enabled = true;
        
        EntranceData.Instance.logAfterLoad = false;
    }
    
    public void LoadSceneFromMenu(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SpawnManager: Nelze načíst scénu - název scény je prázdný.");
            return;
        }
        
        if (EntranceData.Instance != null)
        {
            EntranceData.Instance.lastEntranceKey = string.Empty;
            EntranceData.Instance.logAfterLoad  = false;
        }
        else
        {
            Debug.LogWarning("SpawnManager: EntranceData.Instance je null.");
        }

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(sceneName);
        }
        else
        {
            Debug.LogWarning("SpawnManager: SceneFader.Instance je null, dělám klasické načtení.");
            SceneManager.LoadScene(sceneName);
        }
    }
}
