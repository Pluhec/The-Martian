using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EndingCutsceneEnd : MonoBehaviour
{
    [Header("Nastav v Inspectoru")]    
    public PlayableDirector director;
    public string nextSceneName = "Mars";

    private void OnEnable()
    {
        Debug.Log("EndingCutsceneEnd: OnEnable - subscribing to director.stopped");
        if (director != null)
            director.stopped += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        Debug.Log("EndingCutsceneEnd: OnDisable - unsubscribing from director.stopped");
        if (director != null)
            director.stopped -= OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        Debug.Log($"EndingCutsceneEnd: OnCutsceneFinished called by {pd.name}");

        Debug.Log("EndingCutsceneEnd: Starting DestroyAllDontDestroyObjects");
        DestroyAllDontDestroyObjects();

        Debug.Log("EndingCutsceneEnd: Clearing PlayerPrefs");
        PlayerPrefs.DeleteAll();

        Debug.Log($"EndingCutsceneEnd: Loading next scene '{nextSceneName}'");
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Odstraní všechny objekty v "DontDestroyOnLoad" scéně.
    /// </summary>
    private void DestroyAllDontDestroyObjects()
    {
        Debug.Log("DestroyAllDontDestroyObjects: Creating temp object");
        GameObject tempObject = new GameObject("TempForSceneIdentification");

        Debug.Log("DestroyAllDontDestroyObjects: Marking temp object as DontDestroyOnLoad");
        DontDestroyOnLoad(tempObject);

        Scene dontDestroyScene = tempObject.scene;
        Debug.Log($"DestroyAllDontDestroyObjects: Temp object is in scene '{dontDestroyScene.name}'");

        List<GameObject> rootObjects = new List<GameObject>();
        dontDestroyScene.GetRootGameObjects(rootObjects);
        Debug.Log($"DestroyAllDontDestroyObjects: Found {rootObjects.Count} root objects to destroy");

        foreach (var obj in rootObjects)
        {
            if (obj != tempObject)
            {
                Debug.Log($"DestroyAllDontDestroyObjects: Destroying '{obj.name}'");
                Destroy(obj);
            }
        }

        Debug.Log("DestroyAllDontDestroyObjects: Destroying temp object");
        Destroy(tempObject);
    }
}