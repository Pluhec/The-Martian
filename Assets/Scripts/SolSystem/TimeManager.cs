using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float dayStartTime = 8f;
    public float dayEndTime = 22f;
    private float currentTime;
    public float percentOfDay;

    public bool isTimePaused = false;
    private float realTimeElapsed = 0f;
    private float timeUpdateInterval = 1f;

    public float solDurationInRealMinutes = 10f;

    public event Action<float> WorldTimeChanged; 

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
        ResetTime(); 
    }

    private void Update()
    {
        if (isTimePaused) return;

        float realTimePerGameMinute = solDurationInRealMinutes / (dayEndTime - dayStartTime);
        realTimeElapsed += Time.deltaTime;

        if (realTimeElapsed >= timeUpdateInterval)
        {
            realTimeElapsed = 0f;
            UpdateTime(realTimePerGameMinute);
        }
    }

    public void UpdateTime(float realTimePerGameMinute)
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
                questIndexToPause = activeQuestIndex;
            }
        }

        if (currentTime < dayEndTime)
        {
            currentTime += realTimePerGameMinute;
        }

        if (currentTime > dayEndTime)
        {
            currentTime = dayEndTime;
        }

        
        WorldTimeChanged?.Invoke(currentTime);
        CalculatePercentOfDay(currentTime, dayStartTime, dayEndTime);

        LogCurrentTime();
    }

    public void PauseTime()
    {
        isTimePaused = true;
        Debug.Log("Time paused at: " + currentTime);
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

    public void ResetTime()
    {
        currentTime = dayStartTime;
        Debug.Log("Time reset to 8:00");
        isTimePaused = false;
        questIndexToPause = -1;
        QuestManager.Instance.ResetQuestTimers();
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        return string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    public void LogCurrentTime()
    {
        Debug.Log("Current time: " + GetFormattedTime());
    }

    public void CalculatePercentOfDay(float currentTime, float dayStartTime, float dayEndTime)
    {
        percentOfDay = (currentTime - dayStartTime) / (dayEndTime - dayStartTime) * 100;
        Debug.Log("procento dne: " + percentOfDay);
    }
}