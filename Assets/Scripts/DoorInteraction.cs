using UnityEngine;
using System.Collections.Generic;

public class DoorInteraction : MonoBehaviour, IInteractable {
    public bool isOpen = false;

    public List<InteractionAction> GetInteractions() {
        List<InteractionAction> actions = new List<InteractionAction>();
        InteractionAction action = new InteractionAction();
        action.actionName = isOpen ? "Zavřít" : "Otevřít";
        action.actionCallback = ToggleDoor;
        actions.Add(action);
        return actions;
    }

    public void ToggleDoor() {
        isOpen = !isOpen;
        Debug.Log("Dveře: " + (isOpen ? "Otevřeny" : "Zavřeny"));
        // Zde můžeš přidat animaci nebo zvuk
    }
}