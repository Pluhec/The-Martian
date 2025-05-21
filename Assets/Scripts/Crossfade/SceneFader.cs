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

        // Vytvoř dočasný objekt pro identifikaci DontDestroyOnLoad scény
        GameObject tempObject = new GameObject("TempForSceneIdentification");
        DontDestroyOnLoad(tempObject);
    
        Scene dontDestroyScene = tempObject.scene;
    
        // Získej všechny kořenové objekty
        List<GameObject> rootObjects = new List<GameObject>();
        dontDestroyScene.GetRootGameObjects(rootObjects);
    
        // Prohledej všechny GameObject pro Storage Container
        foreach (var obj in rootObjects)
        {
            StorageContainer[] containers = obj.GetComponentsInChildren<StorageContainer>(true);
            foreach (var container in containers)
            {
                // Přepni aktivní stav - zapnutý vypni a vypnutý zapni
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