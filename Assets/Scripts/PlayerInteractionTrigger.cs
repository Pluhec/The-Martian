using UnityEngine;

public class PlayerInteractionTrigger : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (InteractionManager.Instance == null) {
            Debug.LogError("❌ CHYBA: InteractionManager.Instance je null! Ujisti se, že je správně ve scéně.");
            return;
        }

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable == null) {
            Debug.LogWarning("⚠️ Objekt '" + other.name + "' nemá komponentu IInteractable.");
            return;
        }

        InteractionManager.Instance.SetCurrentInteractable(interactable);
    }


    void OnTriggerExit2D(Collider2D other) {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if(interactable != null) {
            // Pokud hráč opustí oblast interakce, vymažeme aktuální interaktivní objekt
            InteractionManager.Instance.SetCurrentInteractable(null);
        }
    }
}