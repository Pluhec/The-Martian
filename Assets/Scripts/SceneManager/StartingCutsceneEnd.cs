using UnityEngine;
using UnityEngine.Playables;

public class StartingCutsceneEnd : MonoBehaviour
{
    [Header("Nastav v Inspectoru")]
    public PlayableDirector director;
    public string nextSceneName = "Mars";

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}