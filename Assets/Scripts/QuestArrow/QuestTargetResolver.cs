using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class QuestTargetResolver : MonoBehaviour
{
    public static QuestTargetResolver Instance { get; private set; }

    // existující mapa „questID:targetIndex → Transform“
    private Dictionary<string, Transform> targets = new Dictionary<string, Transform>();
    private Transform fallback;
    private Transform terminal;   // <-- nově

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
        => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable()
        => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        => RegisterSceneTargets();

    private void RegisterSceneTargets()
    {
        targets.Clear();
        fallback = null;
        terminal = null;  // <-- reset

        // 1) načti všechna QuestTarget
        foreach (var qt in FindObjectsOfType<QuestTarget>())
        {
            string key = qt.questID + ":" + qt.targetIndex;
            targets[key] = qt.transform;
        }

        // 2) fallback
        var fb = FindObjectOfType<FallbackTarget>();
        if (fb != null) fallback = fb.transform;

        // 3) terminal target
        var tt = FindObjectOfType<TerminalTarget>();
        if (tt != null) terminal = tt.transform;
    }

    public Transform Resolve(int questID, int targetIndex)
    {
        targets.TryGetValue(questID + ":" + targetIndex, out var t);
        return t;
    }

    public Transform FallbackTarget => fallback;
    public Transform TerminalTarget => terminal;  // <-- nově
}