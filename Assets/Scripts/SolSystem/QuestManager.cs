using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Tooltip("Šipku můžeš nechat prázdnou – pokud je null, skript ji sám vyhledá ve scéně.")]
    public QuestArrowPointer arrowPointer;

    private List<Quest> activeQuests = new List<Quest>();
    public List<Quest> ActiveQuests => activeQuests;
    private int currentQuestIndex = 0;

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
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindArrowPointerInScene();
        UpdateArrowTargetAfterSceneLoad();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindArrowPointerInScene();
        UpdateArrowTargetAfterSceneLoad();
    }

    private void FindArrowPointerInScene()
    {
        if (arrowPointer == null)
        {
            arrowPointer = FindObjectOfType<QuestArrowPointer>();
            if (arrowPointer == null)
                Debug.LogWarning("QuestManager: Nenalezl jsem QuestArrowPointer ve scéně " 
                                 + SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateArrowTargetAfterSceneLoad()
    {
        if (activeQuests == null || activeQuests.Count == 0)
            return;

        if (currentQuestIndex >= activeQuests.Count)
        {
            arrowPointer?.SetTarget(null);
            return;
        }

        var quest = activeQuests[currentQuestIndex];
        Transform t = QuestTargetResolver.Instance.Resolve(
            quest.questID, quest.currentTargetIndex
        );
        if (t == null)
            t = QuestTargetResolver.Instance.FallbackTarget;
        arrowPointer?.SetTarget(t);
    }

    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>(quests);
        currentQuestIndex = 0;
        foreach (var q in activeQuests)
            q.isCompleted = false;
        ActivateCurrentQuest();
    }

    private void ActivateCurrentQuest()
    {
        if (currentQuestIndex < activeQuests.Count)
        {
            var quest = activeQuests[currentQuestIndex];
            quest.currentTargetIndex = 0;

            Transform t = QuestTargetResolver.Instance.Resolve(
                quest.questID, quest.currentTargetIndex
            );
            if (t == null)
                t = QuestTargetResolver.Instance.FallbackTarget;

            arrowPointer?.SetTarget(t);
        }
        else
        {
            arrowPointer?.SetTarget(null);
        }
    }

    public void NotifyTargetReached(int questID, Transform reachedTarget)
    {
        var quest = activeQuests.Find(q => q.questID == questID);
        if (quest == null || quest.isCompleted) return;

        // tady se používá stará quest.targets logika, pokud ji nechceš, 
        // stačí spíš volat MarkQuestAsCompletedByID
        if (quest.currentTargetIndex < quest.targets.Count
            && quest.GetCurrentTarget() == reachedTarget)
        {
            quest.currentTargetIndex++;
            if (quest.currentTargetIndex >= quest.targets.Count)
            {
                quest.isCompleted = true;
                Debug.Log($"Quest {quest.questName} (ID:{quest.questID}) completed.");
                TimeManager.Instance.ResumeTime();

                currentQuestIndex++;
                ActivateCurrentQuest();
            }
            else
            {
                Transform t = QuestTargetResolver.Instance.Resolve(
                    quest.questID, quest.currentTargetIndex
                );
                if (t == null)
                    t = QuestTargetResolver.Instance.FallbackTarget;
                arrowPointer?.SetTarget(t);
            }
        }
    }

    /// <summary>
    /// Značí quest questID jako dokončený a pokud to byl právě aktivní quest,
    /// hned se posune na další a aktualizuje šipku.
    /// </summary>
    public void MarkQuestAsCompletedByID(int questID)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            var quest = activeQuests[i];
            if (quest.questID == questID && !quest.isCompleted)
            {
                quest.isCompleted = true;
                Debug.Log($"Quest {quest.questName} (ID:{quest.questID}) completed via MarkQuestAsCompletedByID.");
                TimeManager.Instance.ResumeTime();

                // pokud to byl právě aktivní quest, posuň ukazatel
                if (i == currentQuestIndex)
                {
                    currentQuestIndex++;
                    ActivateCurrentQuest();
                }
                return;
            }
        }
    }

    public void ResetQuestTimers()
    {
        foreach (var quest in activeQuests)
            quest.isCompleted = false;
    }

    public float GetQuestCompletionPercentage()
    {
        int completedCount = 0;
        foreach (var quest in activeQuests)
            if (quest.isCompleted)
                completedCount++;
        return (float)completedCount / activeQuests.Count;
    }

    public bool AreAllQuestsCompleted()
    {
        foreach (var quest in activeQuests)
            if (!quest.isCompleted)
                return false;
        return true;
    }

    public float GetQuestTargetTime()
    {
        int totalQuests = activeQuests.Count;
        float totalDayTime = 14f;
        if (totalQuests == 0)
            return TimeManager.Instance.dayStartTime;

        float timePerQuest = totalDayTime / totalQuests;
        int idx = activeQuests.FindIndex(q => !q.isCompleted);
        if (idx == -1)
            return TimeManager.Instance.dayEndTime;

        return TimeManager.Instance.dayStartTime + (idx + 1) * timePerQuest;
    }
}