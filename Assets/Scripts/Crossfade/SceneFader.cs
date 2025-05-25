using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Animator animator;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string scene)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        GameObject tempObject = new GameObject("TempForSceneIdentification");
        DontDestroyOnLoad(tempObject);

        Scene dontDestroyScene = tempObject.scene;

        List<GameObject> rootObjects = new List<GameObject>();
        dontDestroyScene.GetRootGameObjects(rootObjects);

        foreach (var obj in rootObjects)
        {
            StorageContainer[] containers = obj.GetComponentsInChildren<StorageContainer>(true);
            foreach (var container in containers)
            {
                container.gameObject.SetActive(!container.gameObject.activeSelf);
            }
        }

        Destroy(tempObject);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}