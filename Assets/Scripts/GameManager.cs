using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    [Header("Quest Arrow")]
    public bool arrowEnabled = true;

    private SolSystem solSystem;
    private QuestManager questManager;
    private TimeManager timeManager;
    private bool isPaused = false;

    public bool IsPaused => isPaused;
    public SolSystem SolSystem => solSystem;
    public QuestManager QuestManager => questManager;
    public TimeManager TimeManager => timeManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            solSystem     = FindObjectOfType<SolSystem>();
            questManager  = FindObjectOfType<QuestManager>();
            timeManager   = FindObjectOfType<TimeManager>();

            pauseMenuCanvas.SetActive(false);
            optionsPanel.SetActive(false);
            
            arrowEnabled = PlayerPrefs.GetInt("ArrowEnabled", 1) == 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();

        if (isPaused)
            Input.ResetInputAxes();
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        optionsPanel.SetActive(false);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ToggleOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void UpdateVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void ContinueGame()
    {
        TogglePauseMenu();
    }

    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
    
    public void SetArrowEnabled(bool enabled)
    {
        Debug.Log($"[GameManager] SetArrowEnabled({enabled}) called");
        arrowEnabled = enabled;
        PlayerPrefs.SetInt("ArrowEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        
        var pointers = FindObjectsOfType<QuestArrowPointer>();
        foreach (var p in pointers)
            p.RefreshVisibility();
    }
}