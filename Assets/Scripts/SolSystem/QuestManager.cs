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
        UpdateArrowAfterSceneLoad();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindArrowPointerInScene();
        UpdateArrowAfterSceneLoad();
    }

    /// <summary>
    /// Pokud reference na šipku chybí, najde ji ve scéně.
    /// </summary>
    private void FindArrowPointerInScene()
    {
        if (arrowPointer == null)
        {
            arrowPointer = FindObjectOfType<QuestArrowPointer>();
            if (arrowPointer == null)
                Debug.LogWarning($"QuestManager: nenalezl jsem QuestArrowPointer ve scéně {SceneManager.GetActiveScene().name}");
        }
    }

    /// <summary>
    /// Po načtení scény nebo na startu přenastaví šipku podle stavu questů:
    /// - pokud jsou dokončeny všechny questy → na TerminalTarget
    /// - jinak → na aktuální podquest nebo FallbackTarget
    /// </summary>
    private void UpdateArrowAfterSceneLoad()
    {
        if (activeQuests == null || activeQuests.Count == 0)
            return;

        // 1) pokud jsou všechny questy hotové → ukazuj na terminal
        if (currentQuestIndex >= activeQuests.Count)
        {
            Transform term = QuestTargetResolver.Instance.TerminalTarget;
            arrowPointer?.SetTarget(term);
            return;
        }

        // 2) jinak ukazuj na právě aktivní podquest
        var q = activeQuests[currentQuestIndex];
        Transform t = QuestTargetResolver.Instance.Resolve(q.questID, q.currentTargetIndex)
                     ?? QuestTargetResolver.Instance.FallbackTarget;
        arrowPointer?.SetTarget(t);
    }

    /// <summary>
    /// Inicializuje seznam questů pro nový Sol.
    /// </summary>
    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>(quests);
        currentQuestIndex = 0;
        foreach (var q in activeQuests)
            q.isCompleted = false;

        ActivateCurrentQuest();
    }

    /// <summary>
    /// Nastaví šipku na začátek právě aktivního questu (nebo na terminal).
    /// </summary>
    private void ActivateCurrentQuest()
    {
        if (currentQuestIndex < activeQuests.Count)
        {
            var quest = activeQuests[currentQuestIndex];
            quest.currentTargetIndex = 0;
            Transform t = QuestTargetResolver.Instance.Resolve(quest.questID, quest.currentTargetIndex)
                         ?? QuestTargetResolver.Instance.FallbackTarget;
            arrowPointer?.SetTarget(t);
        }
        else
        {
            // všechny questy hotové → šipka na terminal
            Transform term = QuestTargetResolver.Instance.TerminalTarget;
            arrowPointer?.SetTarget(term);
        }
    }

    /// <summary>
    /// Volá se při dosažení podcíle (pokud používáš Notify).
    /// </summary>
    public void NotifyTargetReached(int questID, Transform reachedTarget)
    {
        var quest = activeQuests.Find(q => q.questID == questID);
        if (quest == null || quest.isCompleted) return;

        if (quest.currentTargetIndex < quest.targets.Count
            && quest.GetCurrentTarget() == reachedTarget)
        {
            quest.currentTargetIndex++;
            if (quest.currentTargetIndex >= quest.targets.Count)
            {
                // dokončení tohoto questu
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
                ) ?? QuestTargetResolver.Instance.FallbackTarget;
                arrowPointer?.SetTarget(t);
            }
        }
    }

    /// <summary>
    /// Označí quest jako dokončený ručně a posune ukazatel,
    /// pokud to byl právě aktivní quest.
    /// </summary>
    public void MarkQuestAsCompletedByID(int questID)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q.questID == questID && !q.isCompleted)
            {
                q.isCompleted = true;
                Debug.Log($"Quest {q.questName} (ID:{q.questID}) completed via MarkQuestAsCompletedByID.");
                TimeManager.Instance.ResumeTime();

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