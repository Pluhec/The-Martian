using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class PathfinderHexInput : MonoBehaviour
{
    [Header("Nastavení")]
    public Transform pointer;                  // Ručička
    public Transform[] hexLabels;              // Cedule 0–F (musí být v pořadí CCW)
    public float rotationStep = 22.5f;         // 360 / 16

    [Header("Výstup")]
    private string currentHexInput = "";
    private string fullMessage = "";

    [Header("Externí odkazy")]
    public Movement playerMovement;
    public CinemachineCamera cmPlayerCam;
    public CinemachineCamera cmPathfinderCam;

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

        RotatePointerTo(currentIndex);
        Debug.Log("Pathfinder aktivní.");
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
            currentHexInput += value;
            Debug.Log($"Vybral jsi: {value}");

            if (currentHexInput.Length == 2)
            {
                int ascii = System.Convert.ToInt32(currentHexInput, 16);
                char character = (char)ascii;
                fullMessage += character;

                Debug.Log($"Znak: '{character}' | Zpráva: \"{fullMessage}\"");
                currentHexInput = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPathfinder();
        }
    }

    private void RotatePointerTo(int index)
    {
        float targetAngle = -90f - (index * rotationStep); // CCW od horní pozice
        pointer.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private string GetCurrentHexValue()
    {
        if (hexLabels == null || hexLabels.Length != 16)
        {
            Debug.LogError("Chybí cedule nebo není správný počet.");
            return "?";
        }

        Transform selected = hexLabels[currentIndex];

        TextMeshProUGUI labelText = selected.GetComponentInChildren<TextMeshProUGUI>();
        if (labelText != null)
        {
            return labelText.text.Trim().ToUpper(); // bezpečný převod
        }

        return selected.name; // fallback, když není text
    }

    private void ExitPathfinder()
    {
        isActive = false;

        // Obnovení ovládání a kamery
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (cmPlayerCam != null)
            cmPlayerCam.Priority = 20;

        if (cmPathfinderCam != null)
            cmPathfinderCam.Priority = 5;

        this.enabled = false;
        Debug.Log("Opustil jsi Pathfinder.");
    }
}
