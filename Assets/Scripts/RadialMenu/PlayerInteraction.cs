using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction2D : MonoBehaviour
{
    public RadialSelection radialSelection;
    private InteractableObject currentObject;
    private Movement playerMovement;

    public float holdThreshold = 0.5f;
    private float holdTime = 0f;
    private bool menuActive = false;
    private bool keyReleased = true;
    private bool actionPerformed = false;
    private bool shouldPlaySound = true;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
        }
        playerMovement = FindObjectOfType<Movement>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (keyReleased)
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
            keyReleased = true;
            holdTime = 0f;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (menuActive)
            {
                HideRadialMenu();
            }
            else if (holdTime < holdThreshold && currentObject != null && !actionPerformed)
            {
                PerformQuickAction();
            }
            actionPerformed = false;
        }
    }

    private void TryOpenRadialMenu()
    {
        if (currentObject == null) return;

        List<string> actions = currentObject.GetActions();

        if (actions.Count >= 2)
        {
            ShowRadialMenu(actions);
            shouldPlaySound = true;
        }
        else if (actions.Count == 1)
        {
            PerformQuickAction();
            actionPerformed = true;
        }

        keyReleased = false;
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
        if (!menuActive)
        {
            return;
        }

        if (radialSelection != null)
        {
            radialSelection.gameObject.SetActive(false);
            menuActive = false;
            if (shouldPlaySound)
            {
                audioManager.PlayRadialMenu(audioManager.openCloseMenu);
            }
        }
    }

    private void PerformRadialAction(int index)
    {
        if (currentObject == null)
        {
            radialSelection.onPartSelected.RemoveListener(PerformRadialAction);
            return;
        }

        List<string> actions = currentObject.GetActions();
        if (index >= 0 && index < actions.Count)
        {
            currentObject.PerformAction(actions[index]);
            radialSelection.onPartSelected.RemoveListener(PerformRadialAction);
            actionPerformed = true;
            shouldPlaySound = false;
            HideRadialMenu();
        }
    }

    public void PerformQuickAction()
    {
        if (currentObject != null)
        {
            audioManager.PlayRadialMenu(audioManager.doAction);
            List<string> actions = currentObject.GetActions();
            if (actions.Count > 0)
            {
                currentObject.PerformAction(actions[0]);
            }
        }
    }

    public void PerformQuickActionInInventory()
    {
        if (currentObject != null)
        {
            audioManager.PlayRadialMenu(audioManager.doAction);
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