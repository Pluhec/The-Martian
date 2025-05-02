using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Spravuje seznam questů, sbírá podcíle z komponent ArrowTarget ve scéně
/// a řídí QuestArrowPointer.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Reference na UI šipku")]
    public QuestArrowPointer arrowPointer;  // drag&drop v Inspectoru

    // seznam všech aktivních questů
    private List<Quest> activeQuests = new List<Quest>();
    private int currentQuestIndex = 0;

    // dočasný slovník: questID → seznam Transformů (podcílů) seřazených podle subTargetIndex
    private Dictionary<int, List<Transform>> targetsByQuestID = new Dictionary<int, List<Transform>>();

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // volá se po každém načtení nové scény
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) vyčisti staré registry
        targetsByQuestID.Clear();

        // 2) nasbírej všechny ArrowTarget komponenty ve scéně
        foreach (var at in FindObjectsOfType<ArrowTarget>())
        {
            if (!targetsByQuestID.ContainsKey(at.questID))
                targetsByQuestID[at.questID] = new List<Transform>();

            targetsByQuestID[at.questID].Add(at.transform);
        }

        // 3) seřaď každý seznam podle subTargetIndex
        foreach (var key in targetsByQuestID.Keys.ToList())
        {
            targetsByQuestID[key] = targetsByQuestID[key]
                .OrderBy(t => t.GetComponent<ArrowTarget>().subTargetIndex)
                .ToList();
        }

        // 4) obnov šipku na správný cíl
        RefreshArrow();
    }

    /// <summary>
    /// Inicializuje seznam questů při startu nového solu
    /// </summary>
    public void InitializeQuests(List<Quest> quests)
    {
        activeQuests = new List<Quest>(quests);
        currentQuestIndex = 0;
        foreach (var q in activeQuests)
            q.isCompleted = false;

        RefreshArrow();
    }

    /// <summary>
    /// Pořídí novou cílovou pozici pro QuestArrowPointer
    /// </summary>
    private void RefreshArrow()
    {
        // pokud už nejsou žádné questy
        if (currentQuestIndex >= activeQuests.Count)
        {
            arrowPointer.SetTarget(null);
            return;
        }

        var quest = activeQuests[currentQuestIndex];
        int idx = quest.currentTargetIndex;

        // pokud existují podcíle v právě načtené scéně
        if (targetsByQuestID.TryGetValue(quest.questID, out var list) && idx < list.Count)
        {
            arrowPointer.SetTarget(list[idx]);
        }
        else
        {
            // fallback-dveře
            var fbGO = GameObject.FindWithTag("QuestFallback");
            arrowPointer.SetTarget(fbGO != null ? fbGO.transform : null);
        }
    }

    /// <summary>
    /// Zavolá se z triggeru, když hráč dosáhne podcíle
    /// </summary>
    public void NotifyTargetReached(int questID, Transform reachedTarget)
    {
        var quest = activeQuests.Find(q => q.questID == questID);
        if (quest == null || quest.isCompleted) return;

        if (targetsByQuestID.TryGetValue(questID, out var list) &&
            quest.currentTargetIndex < list.Count &&
            list[quest.currentTargetIndex] == reachedTarget)
        {
            quest.currentTargetIndex++;
            if (quest.currentTargetIndex >= list.Count)
            {
                quest.isCompleted = true;
                Debug.Log($"Quest {quest.questName} (ID:{quest.questID}) completed.");
                TimeManager.Instance.ResumeTime();
                currentQuestIndex++;
            }
            RefreshArrow();
        }
    }

    /// <summary>
    /// Manuální označení questu jako hotového (např. z UI checkboxu)
    /// </summary>
    public void MarkQuestAsCompletedByID(int questID)
    {
        int i = activeQuests.FindIndex(q => q.questID == questID);
        if (i < 0 || activeQuests[i].isCompleted) return;

        activeQuests[i].isCompleted = true;
        Debug.Log($"Quest {activeQuests[i].questName} (ID:{questID}) manually completed.");
        TimeManager.Instance.ResumeTime();

        if (i == currentQuestIndex)
        {
            currentQuestIndex++;
            RefreshArrow();
        }
    }

    /// <summary>Reset všech questů na nevyřízené</summary>
    public void ResetQuestTimers()
    {
        foreach (var q in activeQuests)
            q.isCompleted = false;
    }

    /// <summary>Procento splněných questů [0..1]</summary>
    public float GetQuestCompletionPercentage()
    {
        int done = activeQuests.Count(q => q.isCompleted);
        return activeQuests.Count == 0 ? 0f : (float)done / activeQuests.Count;
    }

    /// <summary>True, pokud jsou všechny questy splněny</summary>
    public bool AreAllQuestsCompleted()
    {
        return activeQuests.All(q => q.isCompleted);
    }

    /// <summary>Určí čas, kdy se má pauznout podle questů</summary>
    public float GetQuestTargetTime()
    {
        int total = activeQuests.Count;
        if (total == 0) return TimeManager.Instance.dayStartTime;

        float span = TimeManager.Instance.dayEndTime - TimeManager.Instance.dayStartTime;
        float per = span / total;
        int idx = activeQuests.FindIndex(q => !q.isCompleted);
        return idx == -1
            ? TimeManager.Instance.dayEndTime
            : TimeManager.Instance.dayStartTime + (idx + 1) * per;
    }
}