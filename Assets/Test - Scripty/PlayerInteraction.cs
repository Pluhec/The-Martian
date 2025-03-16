using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Nastavení Radial Menu")]
    public RadialSelection radialSelection;
    private InteractableObject currentObject;
    private Movement playerMovement; // Odkaz na pohyb hráče

    [Header("Čas pro dlouhý stisk (otevření menu)")]
    public float holdThreshold = 0.5f;
    private float holdTime = 0f;
    private bool menuActive = false;
    private bool keyReleased = true; // **Nová proměnná pro kontrolu uvolnění klávesy**
    private bool actionPerformed = false; // **Nová proměnná pro kontrolu provedení akce**

    void Start()
    {
        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
        }
        playerMovement = FindObjectOfType<Movement>(); // Najdeme skript Movement
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (keyReleased) // **Ujistíme se, že hráč nejprve pustil "E", než menu otevře znovu**
            {
                holdTime += Time.deltaTime;
                if (holdTime >= holdThreshold && !menuActive && currentObject != null)
                {
                    TryOpenRadialMenu();
                }
            }
        }
        else
        {
            keyReleased = true; // **Uvolnění klávesy umožní další otevření menu**
            holdTime = 0f; // Reset času držení
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (menuActive)
            {
                HideRadialMenu();
            }
            else if (holdTime < holdThreshold && currentObject != null && !actionPerformed)
            {
                PerformQuickAction(); // **Provede rychlou akci, pokud holdTime je menší než holdThreshold**
            }
            actionPerformed = false; // **Reset akce po uvolnění klávesy**
        }
    }

    private void TryOpenRadialMenu()
    {
        if (currentObject == null) return; // Pokud není interaktivní objekt, nic se nestane

        List<string> actions = currentObject.GetActions();

        if (actions.Count >= 2)
        {
            ShowRadialMenu(actions); // Otevřeme menu pouze pokud jsou 2+ akce
        }
        else if (actions.Count == 1)
        {
            PerformQuickAction(); // Pokud je jen 1 akce, rovnou ji provedeme
        }

        keyReleased = false; // **Zamezíme okamžitému znovuotevření**
    }

    private void ShowRadialMenu(List<string> actions)
    {
        if (radialSelection != null)
        {
            radialSelection.SetupMenu(actions);
            radialSelection.onPartSelected.AddListener(PerformRadialAction);
            radialSelection.gameObject.SetActive(true);
            menuActive = true;
        }
    }

    private void HideRadialMenu()
    {
        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
            menuActive = false;
        }
    }

    private void PerformRadialAction(int index)
    {
        List<string> actions = currentObject.GetActions();
        if (index >= 0 && index < actions.Count)
        {
            currentObject.PerformAction(actions[index]);
            radialSelection.onPartSelected.RemoveListener(PerformRadialAction);
            HideRadialMenu();
            actionPerformed = true; // **Nastavíme, že akce byla provedena**
        }
    }

    private void PerformQuickAction()
    {
        if (currentObject != null)
        {
            List<string> actions = currentObject.GetActions();
            if (actions.Count > 0)
            {
                currentObject.PerformAction(actions[0]); // **Provede první akci ihned**
                Debug.Log("Performed quick action: " + actions[0]);
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