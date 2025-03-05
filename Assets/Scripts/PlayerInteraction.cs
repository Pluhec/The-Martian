using UnityEngine;
using SimplePieMenu;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PieMenu pieMenu;
    private bool isNearObject = false;
    private InteractableObject currentObject;
    private bool isHoldingKey = false;
    private float holdTime = 0.5f; // Doba, po kterou je nutné klávesu E držet pro otevření menu
    private float holdTimer = 0f;

    void Update()
    {
        if (isNearObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Stisknuta klávesa E!");
                isHoldingKey = true;
                holdTimer = 0f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime && isHoldingKey)
                {
                    ShowPieMenu();
                    isHoldingKey = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                if (holdTimer < holdTime)
                {
                    QuickAction();
                }
                isHoldingKey = false;
            }
        }
    }

    private void QuickAction()
    {
        if (currentObject != null)
        {
            currentObject.QuickAction();
        }
    }

    private void ShowPieMenu()
    {
        if (pieMenu != null && currentObject != null)
        {
            pieMenu.gameObject.SetActive(true);
            Debug.Log("Pie Menu otevřeno.");
        }
    }

    private void HidePieMenu()
    {
        if (pieMenu != null)
        {
            pieMenu.gameObject.SetActive(false);
        }
    }

    // Detekce vstupu do interakční oblasti
private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Kolize detekována s: " + other.gameObject.name);
    InteractableObject interactable = other.GetComponent<InteractableObject>();
    if (interactable != null)
    {
        Debug.Log("Vstoupil interaktivní objekt");
        isNearObject = true;
        currentObject = interactable;
    }
}


    // Detekce opuštění interakční oblasti
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<InteractableObject>() == currentObject)
        {
            isNearObject = false;
            currentObject = null;
            HidePieMenu();
        }
    }
}