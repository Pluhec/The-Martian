using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // prezistentni systemy nebo manageri kteri ve hre musi zustat za kazdou cenu
    public SolSystem solSystem;
    public QuestManager questManager;
    public TimeManager timeManager;
    
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
        }
    }
}