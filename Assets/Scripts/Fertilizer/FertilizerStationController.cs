using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FertilizerStationController : MonoBehaviour
{
    [Header("UI Root (Canvas), start inactive)")]
    public GameObject uiRoot;

    [Header("Reference na sloty a tlačítko v UI")]
    public InputSlotHandler inputSlot;     // komponenta na levém slotu
    public WorkAreaDropHandler workArea;   // komponenta na pracovní ploše
    public Slot outputSlot;                // pravý dolní slot (běžný Slot)
    public Button unwrapButton;            // tlačítko „Rozbalit“

    [Header("ItemDefinition IDs")]
    public string shitPackID;              // nastav v Inspectoru přesné itemID
    public GameObject compostPrefab;       // prefab Compost (s ItemDefinition & ItemButton)

    // stav mini-hry
    bool hasPackInInput;
    bool hasPackOnWorkArea;

    void Awake()
    {
        Debug.Log("[Station] Awake: disabling UI, hooking up callbacks");
        uiRoot.SetActive(false);
        unwrapButton.interactable = false;

        // propoj callbacky
        inputSlot.station  = this;
        workArea.station   = this;
        unwrapButton.onClick.AddListener(OnClickUnwrap);
    }

    void OnDestroy()
    {
        Debug.Log("[Station] OnDestroy: removing unwrap listener");
        unwrapButton.onClick.RemoveListener(OnClickUnwrap);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Station] OnTriggerEnter2D called with {other.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Station] Player entered station area → showing UI & enabling drag");
            uiRoot.SetActive(true);
            ItemButton.isDragEnabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"[Station] OnTriggerExit2D called with {other.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Station] Player left station area → hiding UI & disabling drag");
            uiRoot.SetActive(false);
            ItemButton.isDragEnabled = false;
            ResetStation();
        }
    }

    public void OnShitPackReceived(ItemButton btn)
    {
        Debug.Log($"[Station] OnShitPackReceived: got {btn.itemID} in inputSlot");
        hasPackInInput = true;
    }

    public void OnWorkAreaDrop(GameObject go)
    {
        Debug.Log($"[Station] OnWorkAreaDrop: object {go.name} dropped onto workArea");
        hasPackOnWorkArea = true;
        unwrapButton.interactable = true;
    }

    void OnClickUnwrap()
    {
        Debug.Log("[Station] OnClickUnwrap: button pressed");
        unwrapButton.interactable = false;

        if (hasPackInInput)
        {
            Debug.Log("[Station] Clearing inputSlot");
            inputSlot.Clear();
            hasPackInInput = false;
        }
        else
        {
            Debug.LogWarning("[Station] OnClickUnwrap called but hasPackInInput is false");
        }

        if (compostPrefab == null)
        {
            Debug.LogError("[Station] compostPrefab is null!");
            return;
        }

        Debug.Log("[Station] Instantiating compostPrefab into outputSlot");
        var obj = Instantiate(compostPrefab);
        outputSlot.OnDirectAdd(obj, 1);
        hasPackOnWorkArea = false;
    }

    void ResetStation()
    {
        Debug.Log("[Station] ResetStation: resetting internal state and UI");
        hasPackInInput = hasPackOnWorkArea = false;
        unwrapButton.interactable = false;
        inputSlot.Clear();
        // outputSlot ponecháme, hráč si může vzít výstup
    }
}