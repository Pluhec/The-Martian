using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Nastavení Radial Menu")]
    public RadialSelection radialSelection;
    private InteractableObject currentObject;

    [Header("Čas pro dlouhý stisk (otevření menu)")]
    public float holdThreshold = 0.5f;
    private float holdTime = 0f;
    private bool menuActive = false;

    void Start()
    {
        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdThreshold && !menuActive && currentObject != null)
            {
                ShowRadialMenu();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (holdTime < holdThreshold && currentObject != null)
            {
                PerformQuickAction();
            }
            holdTime = 0f;
        }
    }

    private void ShowRadialMenu()
    {
        if (radialSelection != null && currentObject != null)
        {
            radialSelection.SetupMenu(currentObject.GetActions());
            radialSelection.onPartSelected.AddListener(PerformRadialAction);
            radialSelection.gameObject.SetActive(true);
            menuActive = true;
        }
    }

    private void PerformRadialAction(int index)
    {
        List<string> actions = currentObject.GetActions();
        if (index >= 0 && index < actions.Count)
        {
            currentObject.PerformAction(actions[index]);
            radialSelection.onPartSelected.RemoveListener(PerformRadialAction); // Odstranění listeneru
            HideRadialMenu();
        }
    }

    public void HideRadialMenu()
    {
        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
            menuActive = false;
            holdTime = 0f;
        }
    }

    private void PerformQuickAction()
    {
        if (currentObject != null)
        {
            List<string> actions = currentObject.GetActions();
            if (actions.Count > 0)
            {
                currentObject.PerformAction(actions[0]);
            }
        }
    }

    private void PerformRadialAction(string action)
    {
        if (currentObject != null)
        {
            currentObject.PerformAction(action);
            HideRadialMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentObject = other.GetComponent<InteractableObject>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentObject = null;
            HideRadialMenu();
        }
    }
}
