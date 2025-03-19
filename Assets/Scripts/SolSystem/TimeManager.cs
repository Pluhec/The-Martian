using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float dayStartTime = 8f; 
    public float dayEndTime = 22f;  

    private float currentTime;
    private bool isTimePaused = false; 

    private float realTimeElapsed = 0f; 
    private float timeUpdateInterval = 1f;
    
    public float timeSpeed = 10f;
    
    private int questIndexToPause = -1;

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

    private void Start()
    {
        currentTime = dayStartTime;
        Debug.Log("Initial time set to: 8:00");
    }

    private void Update()
    {
        if (isTimePaused)
            return; 
        
        realTimeElapsed += Time.deltaTime;
        
        if (realTimeElapsed >= timeUpdateInterval)
        {
            realTimeElapsed = 0f; 
            UpdateTime();
        }
    }

    public void UpdateTime()
    {
        float totalDayTime = dayEndTime - dayStartTime;
        float timePerQuest = totalDayTime / QuestManager.Instance.ActiveQuests.Count;
        
        int activeQuestIndex = QuestManager.Instance.ActiveQuests.FindIndex(q => !q.isCompleted);
        if (activeQuestIndex != -1)
        {
            float targetTimeForQuest = dayStartTime + timePerQuest * (activeQuestIndex + 1);

            if (currentTime >= targetTimeForQuest && !isTimePaused)
            {
                isTimePaused = true; 
                Debug.Log("Time paused at: " + currentTime);
                questIndexToPause = activeQuestIndex;
            }
        }
        
        if (currentTime < dayEndTime)
        {
            currentTime += (timePerQuest * timeSpeed);
        }

        
        if (currentTime > dayEndTime)
        {
            currentTime = dayEndTime;
        }
        
        LogCurrentTime();
    }

    public void ResumeTime()
    {
        if (questIndexToPause != -1 && QuestManager.Instance.ActiveQuests[questIndexToPause].isCompleted)
        {
            isTimePaused = false;
            questIndexToPause = -1;
            Debug.Log("Time resumed at: " + currentTime);
        }
    }
    
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        float seconds = (currentTime - hours - minutes / 60f) * 3600f;
        return string.Format("{0:D2}:{1:D2}:{2:F2}", hours, minutes, seconds);
    }

    public void LogCurrentTime()
    {
        Debug.Log("Current time: " + GetFormattedTime());
    }
}