using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public float dayStartTime = 8f;  // Začátek Solu (8:00)
    public float dayEndTime = 22f;   // Konec Solu (22:00)

    private float currentTime;  // Počáteční čas
    private float timeFlowCoefficient = 1f; // Počáteční koeficient (normální rychlost)
    private float lastQuestCompletionPercentage = 0f;

    private float realTimeElapsed = 0f; // Real time tracking
    private float timeUpdateInterval = 1f; // 1 second interval to update the time

    // Koeficient rychlosti plynutí času (např. 10x rychlejší než skutečný čas)
    public float timeSpeed = 10f;

    private bool isTimePaused = false; // Flag to pause/unpause time when quest is not completed

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
        // Inicializace s časem 8:00
        currentTime = dayStartTime; // Začínáme na 8:00
        Debug.Log("Initial time set to: 8:00");
    }

    private void Update()
    {
        if (isTimePaused)
            return; // Pokud je čas pozastaven, neaktualizujeme čas

        // Sledujeme uplynulý čas každou reálnou sekundu
        realTimeElapsed += Time.deltaTime;

        // Pokud uplynula jedna sekunda, aktualizujeme čas
        if (realTimeElapsed >= timeUpdateInterval)
        {
            realTimeElapsed = 0f; // Resetuj realTimeElapsed pro další sekundu
            UpdateTime(QuestManager.Instance.GetQuestCompletionPercentage());
        }
    }

    public void UpdateTime(float questCompletionPercentage)
    {
        // Dynamický výpočet pro zastavení času podle počtu splněných questů
        float questPercentage = questCompletionPercentage;

        // Rozdělíme celkový čas (14 hodin) mezi všechny questy
        float totalDayTime = dayEndTime - dayStartTime;
        float timePerQuest = totalDayTime / QuestManager.Instance.ActiveQuests.Count;

        // Pokud máme questy, spočítáme cílový čas
        float targetTimeForQuest = dayStartTime + timePerQuest * QuestManager.Instance.ActiveQuests.FindIndex(q => !q.isCompleted);

        // Dynamické zpomalování plynutí času podle splněných questů
        float timeDelta = timePerQuest * Mathf.Max(0.01f, questPercentage) * timeFlowCoefficient;

        // Přičítáme s koeficientem rychlosti plynutí času
        timeDelta *= timeSpeed;

        currentTime += timeDelta;

        // Zastavení času podle dynamického výpočtu
        if (currentTime >= targetTimeForQuest)
        {
            isTimePaused = true; // Pauza pro čas
            Debug.Log("Time paused at: " + currentTime);
        }

        // Zajistí, že čas nikdy nevybočí mimo hranice (8:00 - 22:00)
        if (currentTime > dayEndTime)
        {
            currentTime = dayEndTime;
        }

        // Log pro sledování aktuálního času
        LogCurrentTime();

        // Zajištění, že čas není nižší než denní začátek
        if (currentTime < dayStartTime)
        {
            currentTime = dayStartTime;
        }
    }

    // Formátování času na "HH:MM:SS" (např. 8:03, 8:03.95)
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        float seconds = (currentTime - hours - minutes / 60f) * 3600f; // Vypočítáme zbytek pro sekundy
        return string.Format("{0:D2}:{1:D2}:{2:F2}", hours, minutes, seconds);
    }

    // Veřejná vlastnost pro přístup k hodnotě currentTime
    public float CurrentTime
    {
        get { return currentTime; }
    }

    // Logování aktuálního času každou sekundu
    public void LogCurrentTime()
    {
        Debug.Log("Current time: " + GetFormattedTime());
    }

    // Obnovit čas, pokud je quest splněn
    public void ResumeTime()
    {
        isTimePaused = false;
        Debug.Log("Time resumed at: " + currentTime);
    }
}