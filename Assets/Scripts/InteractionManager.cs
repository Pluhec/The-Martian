using UnityEngine;
using System.Collections.Generic;
using SimplePieMenu;
using UnityEngine.UI;
using TMPro;

public class InteractionManager : MonoBehaviour {
    public static InteractionManager Instance;

    [Tooltip("Čas pro zobrazení Pie Menu při podržení E")]
    public float holdTimeThreshold = 0.5f;
    private float keyHoldTime = 0f;
    private bool isHolding = false;
    
    // Reference z assetu Simple Pie Menu – přiřaď v Inspectoru
    public PieMenuDisplayer pieMenuDisplayer; // Komponenta z Pie Menu prefab
    public PieMenu pieMenu;                   // Samotné Pie Menu
    public GameObject defaultMenuItemPrefab;  // Prefab pro položku menu (musí mít podobjekty "Label" a "Icon")
    
    // Aktuální interaktivní objekt (implementuje IInteractable)
    private IInteractable currentInteractable;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            keyHoldTime = 0f;
            isHolding = true;
        }
        if (Input.GetKey(KeyCode.E) && isHolding) {
            keyHoldTime += Time.deltaTime;
            if (keyHoldTime >= holdTimeThreshold) {
                if (currentInteractable != null) {
                    List<InteractionAction> actions = currentInteractable.GetInteractions();
                    ShowPieMenu(actions);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.E) && isHolding) {
            isHolding = false;
            if (keyHoldTime < holdTimeThreshold) {
                // Quick action: spustí se první akce
                if (currentInteractable != null) {
                    List<InteractionAction> actions = currentInteractable.GetInteractions();
                    if (actions.Count > 0) {
                        actions[0].actionCallback?.Invoke();
                    }
                }
            } else {
                ClosePieMenu();
            }
        }
    }
    
    public void SetCurrentInteractable(IInteractable interactable) {
        currentInteractable = interactable;
    }
    
    // Vytvoření položek menu a zobrazení Pie Menu
    private void ShowPieMenu(List<InteractionAction> actions) {
        List<GameObject> menuItemsList = new List<GameObject>();
        foreach (var action in actions) {
            // Vytvoříme instanci prefab položky
            GameObject menuItem = Instantiate(defaultMenuItemPrefab);
            
            // Nastavíme text – najdeme podobjekt "Label" a získáme TextMeshProUGUI
            Transform labelTransform = menuItem.transform.Find("Label");
            if (labelTransform != null) {
                TextMeshProUGUI label = labelTransform.GetComponent<TextMeshProUGUI>();
                if (label != null) {
                    label.text = action.actionName;
                }
            }
            
            // Nastavíme ikonu – najdeme podobjekt "Icon" a získáme Image
            Transform iconTransform = menuItem.transform.Find("Icon");
            if (iconTransform != null) {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null && action.actionIcon != null) {
                    iconImage.sprite = action.actionIcon;
                }
            }
            
            // Přidáme click handler – použijeme Button komponentu
            Button btn = menuItem.GetComponent<Button>();
            if (btn != null) {
                btn.onClick.AddListener(() => {
                    action.actionCallback?.Invoke();
                    ClosePieMenu();
                });
            }
            
            menuItemsList.Add(menuItem);
        }
        
        // Přidáme položky do Pie Menu pomocí MenuItemAdder dle dokumentace (viz kapitola 4.8)
        MenuItemAdder adder = PieMenuShared.References.MenuItemsManager.MenuItemAdder;
        adder.Add(pieMenu, menuItemsList);
        
        // Zobrazíme Pie Menu (viz kapitola 4.3)
        pieMenuDisplayer.ShowPieMenu(pieMenu);
    }
    
    // Zavření menu – jednoduše deaktivujeme Pie Menu GameObject
    private void ClosePieMenu() {
        pieMenu.gameObject.SetActive(false);
    }
}