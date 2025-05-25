using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class QuestTargetResolver : MonoBehaviour
{
    public static QuestTargetResolver Instance { get; private set; }
    
    private Dictionary<string, Transform> targets = new Dictionary<string, Transform>();
    private Transform fallback;
    private Transform terminal;

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
        terminal = null;

        foreach (var qt in FindObjectsOfType<QuestTarget>())
        {
            string key = qt.questID + ":" + qt.targetIndex;
            targets[key] = qt.transform;
        }

        var fb = FindObjectOfType<FallbackTarget>();
        if (fb != null) fallback = fb.transform;

        var tt = FindObjectOfType<TerminalTarget>();
        if (tt != null) terminal = tt.transform;
    }

    public Transform Resolve(int questID, int targetIndex)
    {
        targets.TryGetValue(questID + ":" + targetIndex, out var t);
        return t;
    }

    public Transform FallbackTarget => fallback;
    public Transform TerminalTarget => terminal;
}