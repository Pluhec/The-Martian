using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasCameraUpdater : MonoBehaviour {
    private Canvas canvas;
    
    void Awake() {
        canvas = GetComponent<Canvas>();
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
            canvas.worldCamera = Camera.main;
        }
    }
}
