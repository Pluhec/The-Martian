using UnityEngine;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour {
    public static InteractionManager Instance;

    [Tooltip("Čas, po kterém se při podržení E zobrazí radial menu")]
    public float holdTimeThreshold = 0.5f;
    private float keyHoldTime = 0f;
    private bool isHolding = false;

    [Header("UI Reference")]
    public RadialMenu radialMenu;  // Přetáhni sem objekt s komponentou RadialMenu

    // Aktuální interaktivní objekt, na který se dívá hráč nebo je v dosahu
    private IInteractable currentInteractable;

    void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        // Sleduj stisk klávesy E
        if(Input.GetKeyDown(KeyCode.E)) {
            keyHoldTime = 0f;
            isHolding = true;
        }
        if(Input.GetKey(KeyCode.E) && isHolding) {
            keyHoldTime += Time.deltaTime;
            if(keyHoldTime >= holdTimeThreshold && !radialMenu.IsVisible) {
                // Zobraz radial menu
                if(currentInteractable != null) {
                    List<InteractionAction> actions = currentInteractable.GetInteractions();
                    radialMenu.Show(actions);
                }
            }
        }
        if(Input.GetKeyUp(KeyCode.E) && isHolding) {
            isHolding = false;
            if(keyHoldTime < holdTimeThreshold) {
                // Quick action: pokud je definována aspoň jedna akce, vykonej první
                if(currentInteractable != null) {
                    List<InteractionAction> actions = currentInteractable.GetInteractions();
                    if(actions.Count > 0) {
                        actions[0].actionCallback?.Invoke();
                    }
                }
            } else {
                // Po uvolnění klávesy skryj menu
                radialMenu.Hide();
            }
        }
    }

    // Tuto metodu voláme, když se hráč přiblíží k interaktivnímu objektu
    public void SetCurrentInteractable(IInteractable interactable) {
        currentInteractable = interactable;
    }
}