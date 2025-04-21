using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class SpawnPoint
{
    public string key;
    public Transform location;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Seznam všech spawnu ve scéně.")]
    public SpawnPoint[] spawnPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // po nacteni sceny port na transition ktery mu trigger predal
        TeleportPlayer(EntranceData.Instance.lastEntranceKey);
    }
    
    public void TeleportPlayer(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        var player = GameObject.FindWithTag("Player");
        if (player == null) 
        {
            Debug.Log($"SpawnManager: Hráč nenalezen pro teleport '{key}'.");
            return;
        }

        var sp = spawnPoints.FirstOrDefault(p => p.key == key);
        if (sp != null && sp.location != null)
        {
            player.transform.position = sp.location.position;
        }
        else
        {
            Debug.Log($"SpawnManager: Klíč '{key}' nenašel žádný spawnPoint.");
        }
    }
}