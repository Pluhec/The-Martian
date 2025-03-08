using UnityEngine;
using System.Collections.Generic;


public class PlayerInteraction2D : MonoBehaviour
{
    public GameObject radialMenuCanvas;
    private InteractableObject currentObject;
    private float holdTime = 0f;
    private float holdThreshold = 0.5f; // Po jaké době se otevře menu
    private bool menuActive = false;
    private bool isQuickAction = false;

    void Start()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(false); // Menu je na začátku skryté
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E)) // Hráč drží klávesu
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdThreshold && !menuActive && currentObject != null)
            {
                ShowRadialMenu();
                isQuickAction = false; // Pokud se otevře menu, quick action se neprovede
            }
        }

        if (Input.GetKeyUp(KeyCode.E)) // Hráč pustil klávesu
        {
            if (holdTime < holdThreshold && currentObject != null)
            {
                PerformQuickAction();
            }

            holdTime = 0f; // Resetujeme čas držení klávesy
        }
    }

    private void ShowRadialMenu()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(true);
            RadialMenu menu = radialMenuCanvas.GetComponent<RadialMenu>();
            menu.SetupMenu(currentObject);
            menuActive = true;
        }
    }

    private void HideRadialMenu()
    {
        if (radialMenuCanvas != null)
        {
            radialMenuCanvas.SetActive(false);
            menuActive = false;
        }
    }

    private void PerformQuickAction()
    {
        if (currentObject != null)
        {
            List<string> actions = currentObject.GetActions();
            if (actions.Count > 0)
            {
                currentObject.PerformAction(actions[0]); // Provede první akci v seznamu
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
