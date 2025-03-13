using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Privátní reference na persistentní systémy
    private SolSystem solSystem;
    private QuestManager questManager;
    private TimeManager timeManager;

    // Veřejné čtecí vlastnosti, pokud je potřebuješ v jiných skriptech
    public SolSystem SolSystem { get { return solSystem; } }
    public QuestManager QuestManager { get { return questManager; } }
    public TimeManager TimeManager { get { return timeManager; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // dynamicky se najdou scripty
            solSystem = FindObjectOfType<SolSystem>();
            questManager = FindObjectOfType<QuestManager>();
            timeManager = FindObjectOfType<TimeManager>();

            if (solSystem == null)
                Debug.LogWarning("GameManager: SolSystem nebyl nalezen!");
            if (questManager == null)
                Debug.LogWarning("GameManager: QuestManager nebyl nalezen!");
            if (timeManager == null)
                Debug.LogWarning("GameManager: TimeManager nebyl nalezen!");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}