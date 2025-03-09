// using UnityEngine;
// using System.Collections.Generic;
//
// public class InteractionManager : MonoBehaviour {
//     public static InteractionManager Instance { get; private set; }
//
//     void Awake() {
//         if (Instance == null) {
//             Instance = this;
//         } else {
//             Debug.LogError("❌ CHYBA: Ve scéně existuje více než jeden InteractionManager!");
//             Destroy(gameObject);
//         }
//
//         // Automaticky vyhledá RadialMenu ve scéně
//         if (radialMenu == null) {
//             radialMenu = FindObjectOfType<RadialMenu>();
//             if (radialMenu == null) {
//                 Debug.LogError("❌ CHYBA: RadialMenu nebylo nalezeno ve scéně! Ujisti se, že existuje v Hierarchy.");
//             }
//         }
//     }
//
//
//     [Tooltip("Čas, po kterém se při podržení E zobrazí radial menu")]
//     public float holdTimeThreshold = 0.5f;
//     private float keyHoldTime = 0f;
//     private bool isHolding = false;
//
//     [Header("UI Reference")]
//     public RadialMenu radialMenu;  // Přetáhni sem objekt s komponentou RadialMenu
//
//     // Aktuální interaktivní objekt, na který se dívá hráč nebo je v dosahu
//     private IInteractable currentInteractable;
//
//     void Update() {
//         if (radialMenu == null) {
//             Debug.LogError("❌ CHYBA: RadialMenu není nastaveno v `InteractionManager`!");
//             return;
//         }
//
//         if (Input.GetKeyDown(KeyCode.E)) {
//             keyHoldTime = 0f;
//             isHolding = true;
//         }
//
//         if (Input.GetKey(KeyCode.E) && isHolding) {
//             keyHoldTime += Time.deltaTime;
//             if (keyHoldTime >= holdTimeThreshold && !radialMenu) {
//                 if (currentInteractable != null) {
//                     List<InteractionAction> actions = currentInteractable.GetInteractions();
//                     if (actions.Count > 0) {
//                     }
//                 }
//             }
//         }
//
//         if (Input.GetKeyUp(KeyCode.E) && isHolding) {
//             isHolding = false;
//             if (keyHoldTime < holdTimeThreshold) {
//                 if (currentInteractable != null) {
//                     List<InteractionAction> actions = currentInteractable.GetInteractions();
//                     if (actions.Count > 0) {
//                         actions[0].actionCallback?.Invoke();
//                     }
//                 }
//             }
//         }
//     }
//
//
//     // Tuto metodu voláme, když se hráč přiblíží k interaktivnímu objektu
//     public void SetCurrentInteractable(IInteractable interactable) {
//         currentInteractable = interactable;
//     }
// }