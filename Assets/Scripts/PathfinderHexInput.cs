using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class PathfinderHexInput : MonoBehaviour
{
    [Header("Nastavení")]
    public Transform pointer;                  // Ručička
    public Transform[] hexLabels;              // Cedule 0–F (musí být v pořadí CCW)
    public float rotationStep = 22.5f;         // 360 / 16

    [Header("Výstup")]
    private string currentHexInput = "";
    private string fullMessage = "";
    private bool isFirstCharacter = true;

    [Header("Externí odkazy")]
    public Movement playerMovement;
    public CinemachineCamera cmPlayerCam;
    public CinemachineCamera cmPathfinderCam;
    public GameObject pathfinderCanvas;
    public GameObject playerUICanvas;      // Add this
    public GameObject inventoryCanvas;     // Add this
    public GameObject questUICanvas;

    [Header("Zpráva")]
    public TextMeshProUGUI messageDisplay;
    public string targetMessage = "HELLO";

    private int currentCharIndex = 0;
    private bool[] correctChars;
    private bool[] attemptedChars;

    private int currentIndex = 0;
    private bool isActive = false;

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

        currentCharIndex = 0;
        correctChars = new bool[targetMessage.Length];
        attemptedChars = new bool[targetMessage.Length];

        // Show pathfinder UI, hide other UIs
        if (pathfinderCanvas != null)
            pathfinderCanvas.SetActive(true);
        if (playerUICanvas != null)
            playerUICanvas.SetActive(false);
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
        if (questUICanvas != null)
            questUICanvas.SetActive(false);

        UpdateMessageDisplay();
        Debug.Log("Pathfinder aktivní.");
    }

    private void UpdateMessageDisplay()
    {
        string result = "";

        for (int i = 0; i < targetMessage.Length; i++)
        {
            char c = targetMessage[i];

            if (i < currentCharIndex)
            {
                result += correctChars[i]
                    ? $"<color=green>{c}</color>"
                    : $"<color=red>{c}</color>";
            }
            else if (i == currentCharIndex && attemptedChars[i])
            {
                result += correctChars[i]
                    ? $"<color=green>{c}</color>"
                    : $"<color=red>{c}</color>";
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

                Debug.Log($"Zadal jsi znak: {enteredChar}");

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

                    if (currentCharIndex >= targetMessage.Length)
                    {
                        if (System.Array.TrueForAll(correctChars, c => c))
                        {
                            Debug.Log("✅ Zpráva byla správně zadána!");
                        }
                    }
                }

                currentHexInput = "";
                isFirstCharacter = true;
            }

            Debug.Log($"Vybral jsi: {value}");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPathfinder();
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

        // Hide pathfinder UI, show other UIs
        if (pathfinderCanvas != null)
            pathfinderCanvas.SetActive(false);
        if (playerUICanvas != null)
            playerUICanvas.SetActive(true);
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(true);
        if (questUICanvas != null)
            questUICanvas.SetActive(true);

        this.enabled = false;
        Debug.Log("Opustil jsi Pathfinder.");
    }
}