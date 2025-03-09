using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Nastavení Radial Menu")]
    public GameObject radialMenuCanvas;
    private InteractableObject currentObject;

    [Header("Čas pro dlouhý stisk (otevření menu)")]
    public float holdThreshold = 0.5f;
    private float holdTime = 0f;
    private bool menuActive = false;

    void Start()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(false);
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
            // zavolani quick action podle delky drzeni klavesy
            if (holdTime < holdThreshold && currentObject != null)
            {
                PerformQuickAction();
            }
            holdTime = 0f; // restart casu
        }
    }

    private void ShowRadialMenu()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(true);
            RadialMenu menu = radialMenuCanvas.GetComponent<RadialMenu>();
            if (menu != null)
            {
                menu.SetupMenu(currentObject);
                menuActive = true;
            }
            else
            {
                Debug.LogError("PlayerInteraction2D: RadialMenu komponenta nebyla nalezena!");
            }
        }
    }

    public void HideRadialMenu()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(false);
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