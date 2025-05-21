using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class PathfinderHexInput : MonoBehaviour
{
    [Header("Nastavení")]
    public Transform pointer;
    public Transform[] hexLabels;
    public float rotationStep = 22.5f;

    [Header("Výstup")]
    private string currentHexInput = "";
    private string fullMessage = "";
    private bool isFirstCharacter = true;

    [Header("Externí odkazy")]
    public Movement playerMovement;
    public CinemachineCamera cmPlayerCam;
    public CinemachineCamera cmPathfinderCam;
    public GameObject pathfinderCanvas;
    public GameObject playerUICanvas;
    public GameObject inventoryCanvas;
    public GameObject questUICanvas;

    [Header("Zpráva")]
    public TextMeshProUGUI messageDisplay;
    [SerializeField] private PathfinderMessage[] questMessages;

    [Header("Quest Settings")]
    public int minQuestID = 10;
    public int maxQuestID = 20;
    public int currentQuestID = 10;

    [System.Serializable]
    public class PathfinderMessage
    {
        public int questID;
        public string message;
    }

    private string targetMessage = "TEST";
    private int currentCharIndex = 0;
    private bool[] correctChars;
    private bool[] attemptedChars;
    private int currentIndex = 0;
    private bool isActive = false;
    private bool tutorialShown = false;
    private bool questCompleted = false;
    private QuestManager questManager;
    private GameObject toastPrefab;
    private Transform notificationsParent;

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            questManager = GameManager.Instance.QuestManager;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }

        InitializeDefaultMessages();
        
        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }
        else
        {
            Debug.LogWarning("Notification canvas s tagem 'notificationSystem' nebyl nalezen.");
        }
    }

    private void InitializeDefaultMessages()
    {
        if (questMessages == null || questMessages.Length == 0)
        {
            questMessages = new PathfinderMessage[]
            {
                new PathfinderMessage { questID = 10, message = "HELLO" },
                new PathfinderMessage { questID = 12, message = "OXYGEN" },
                new PathfinderMessage { questID = 14, message = "SURVIVE" },
                new PathfinderMessage { questID = 16, message = "MARS" },
                new PathfinderMessage { questID = 18, message = "RESCUE" }
            };
        }
    }

    private void Update()
    {
        if (!isActive) return;
        HandleInput();
    }

    public void StartPathfinder()
    {
        isActive = true;
        currentHexInput = "";
        fullMessage = "";
        currentIndex = 0;
        isFirstCharacter = true;
        pointer.rotation = Quaternion.identity;
        questCompleted = false;

        foreach (Quest quest in questManager.ActiveQuests)
        {
            if (!quest.isCompleted && quest.questID >= minQuestID &&
                quest.questID <= maxQuestID && quest.questID % 2 == 0)
            {
                currentQuestID = quest.questID;
                var message = System.Array.Find(questMessages, m => m.questID == currentQuestID);
                targetMessage = message != null ? message.message : "TEST";
                break;
            }
        }

        currentCharIndex = 0;
        correctChars = new bool[targetMessage.Length];
        attemptedChars = new bool[targetMessage.Length];

        if (pathfinderCanvas != null)
            pathfinderCanvas.SetActive(true);
        if (playerUICanvas != null)
            playerUICanvas.SetActive(false);
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
        if (questUICanvas != null)
            questUICanvas.SetActive(false);

        if (!tutorialShown && toastPrefab != null && notificationsParent != null)
        {
            var tutorial = Instantiate(toastPrefab, notificationsParent);
            tutorial.GetComponent<Toast>().Show("info", "Controls: A/D = Rotate | SPACE = Confirm | F = Exit");
            tutorialShown = true;
        }

        UpdateMessageDisplay();
        Debug.Log($"Pathfinder aktivní. Quest ID: {currentQuestID}, Message: {targetMessage}");
    }

    private void UpdateMessageDisplay()
    {
        string result = "";
        for (int i = 0; i < targetMessage.Length; i++)
        {
            char c = targetMessage[i];
            if (i < currentCharIndex)
            {
                result += correctChars[i] ? $"<color=green>{c}</color>" : $"<color=red>{c}</color>";
            }
            else if (i == currentCharIndex && attemptedChars[i])
            {
                result += correctChars[i] ? $"<color=green>{c}</color>" : $"<color=red>{c}</color>";
            }
            else
            {
                result += c;
            }
        }
        if (messageDisplay != null)
            messageDisplay.text = result;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex = (currentIndex - 1 + 16) % 16;
            RotatePointerTo(currentIndex);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex = (currentIndex + 1) % 16;
            RotatePointerTo(currentIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            string value = GetCurrentHexValue();

            if (isFirstCharacter)
            {
                currentHexInput = value;
                isFirstCharacter = false;
            }
            else
            {
                currentHexInput += value;
                int asciiValue = int.Parse(currentHexInput, System.Globalization.NumberStyles.HexNumber);
                char enteredChar = (char)asciiValue;

                if (currentCharIndex < targetMessage.Length)
                {
                    char expectedChar = targetMessage[currentCharIndex];
                    attemptedChars[currentCharIndex] = true;

                    if (enteredChar == expectedChar)
                    {
                        correctChars[currentCharIndex] = true;
                        currentCharIndex++;
                    }
                    else
                    {
                        correctChars[currentCharIndex] = false;
                    }

                    UpdateMessageDisplay();

                    if (currentCharIndex >= targetMessage.Length && System.Array.TrueForAll(correctChars, c => c))
                    {
                        CompletePathfinderQuest();
                    }
                }

                currentHexInput = "";
                isFirstCharacter = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ExitPathfinder();
        }
    }

    private void CompletePathfinderQuest()
    {
        Quest quest = questManager.ActiveQuests.Find(q => q.questID == currentQuestID);
        if (quest != null && !quest.isCompleted)
        {
            questManager.MarkQuestAsCompletedByID(currentQuestID);
            questCompleted = true;

            if (toastPrefab != null && notificationsParent != null)
            {
                var toast = Instantiate(toastPrefab, notificationsParent);
                toast.GetComponent<Toast>().Show("info", "Message successfully entered! Go to the terminal and enter the command /message.");
            }
        }
    }

    private void RotatePointerTo(int index)
    {
        float targetAngle = -(index * rotationStep);
        pointer.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private string GetCurrentHexValue()
    {
        return currentIndex.ToString("X");
    }

    private void ExitPathfinder()
    {
        isActive = false;

        if (playerMovement != null)
            playerMovement.enabled = true;
        if (cmPlayerCam != null)
            cmPlayerCam.Priority = 20;
        if (cmPathfinderCam != null)
            cmPathfinderCam.Priority = 5;
        if (pathfinderCanvas != null)
            pathfinderCanvas.SetActive(false);
        if (playerUICanvas != null)
            playerUICanvas.SetActive(true);
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(true);
        if (questUICanvas != null)
            questUICanvas.SetActive(true);

        this.enabled = false;
    }
}