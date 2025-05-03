using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Drží mapu všech QuestTarget a jednoho FallbackTarget
/// v právě aktivní scéně.
/// </summary>
public class QuestTargetResolver : MonoBehaviour
{
    public static QuestTargetResolver Instance { get; private set; }

    // klíč je "questID:targetIndex"
    private Dictionary<string, Transform> targets = new Dictionary<string, Transform>();
    private Transform fallback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterSceneTargets();
    }

    private void RegisterSceneTargets()
    {
        targets.Clear();
        fallback = null;

        foreach (var qt in FindObjectsOfType<QuestTarget>())
        {
            string key = qt.questID + ":" + qt.targetIndex;
            targets[key] = qt.transform;
        }

        var fb = FindObjectOfType<FallbackTarget>();
        if (fb != null)
            fallback = fb.transform;
    }

    /// <summary>
    /// Vrátí Transform pro daný questID + targetIndex,
    /// nebo null, pokud v dané scéně neexistuje.
    /// </summary>
    public Transform Resolve(int questID, int targetIndex)
    {
        string key = questID + ":" + targetIndex;
        targets.TryGetValue(key, out var t);
        return t;
    }

    /// <summary>
    /// Vrátí transform z FallbackTarget (nebo null).
    /// </summary>
    public Transform FallbackTarget => fallback;
}