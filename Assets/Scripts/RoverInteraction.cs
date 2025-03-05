using UnityEngine;
using System.Collections.Generic;

public class RoverInteraction : MonoBehaviour, IInteractable {
    public List<InteractionAction> GetInteractions() {
        List<InteractionAction> actions = new List<InteractionAction>();

        // Quick action: nasednout
        InteractionAction quickAction = new InteractionAction();
        quickAction.actionName = "Nasednout";
        quickAction.actionCallback = () => {
            Debug.Log("Nasednuto");
            // Zde implementuj logiku nasednutí
        };
        quickAction.header = "Rover";
        quickAction.description = "Nasednout na rover";
        // quickAction.actionIcon = /* přiřaď ikonu */;
        actions.Add(quickAction);

        // Druhá akce: Nabíjet
        InteractionAction chargeAction = new InteractionAction();
        chargeAction.actionName = "Nabíjet";
        chargeAction.actionCallback = () => {
            Debug.Log("Nabíjení");
            // Zde implementuj logiku nabíjení
        };
        chargeAction.header = "Rover";
        chargeAction.description = "Nabíjet rover";
        // chargeAction.actionIcon = /* přiřaď ikonu */;
        actions.Add(chargeAction);

        // Třetí akce: Vyměnit pneumatiku
        InteractionAction tireAction = new InteractionAction();
        tireAction.actionName = "Vyměnit pneumatiku";
        tireAction.actionCallback = () => {
            Debug.Log("Pneumatika vyměněna");
            // Zde implementuj logiku výměny pneu
        };
        tireAction.header = "Rover";
        tireAction.description = "Vyměnit pneumatiku na roveru";
        // tireAction.actionIcon = /* přiřaď ikonu */;
        actions.Add(tireAction);

        return actions;
    }
}