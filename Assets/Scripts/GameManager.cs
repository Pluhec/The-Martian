using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private SolSystem solSystem;
    private QuestManager questManager;
    private TimeManager timeManager;
    private bool isPaused = false;
    
    public bool IsPaused { get { return isPaused; } }

    public SolSystem SolSystem { get { return solSystem; } }
    public QuestManager QuestManager { get { return questManager; } }
    public TimeManager TimeManager { get { return timeManager; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            solSystem = FindObjectOfType<SolSystem>();
            questManager = FindObjectOfType<QuestManager>();
            timeManager = FindObjectOfType<TimeManager>();
            
            pauseMenuCanvas.SetActive(false);
            optionsPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        
        if (isPaused)
        {
            Input.ResetInputAxes();
        }
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
}