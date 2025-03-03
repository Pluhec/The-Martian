using UnityEngine;

public class PlayerInteractionTrigger : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if(interactable != null) {
            InteractionManager.Instance.SetCurrentInteractable(interactable);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if(interactable != null) {
            // Pokud hráč opustí oblast interakce, vymažeme aktuální interaktivní objekt
            InteractionManager.Instance.SetCurrentInteractable(null);
        }
    }
}