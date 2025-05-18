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
    public string shitPackID;   // nastav v Inspectoru přesné itemID z ItemDefinition
    public GameObject compostPrefab; // prefab Compost (s ItemDefinition & ItemButton)

    // stav mini-hry
    bool hasPackInInput;
    bool hasPackOnWorkArea;

    void Awake()
    {
        uiRoot.SetActive(false);
        unwrapButton.interactable = false;

        // propoj callbacky
        inputSlot.station  = this;
        workArea.station   = this;
        unwrapButton.onClick.AddListener(OnClickUnwrap);
    }

    void OnDestroy()
    {
        unwrapButton.onClick.RemoveListener(OnClickUnwrap);
    }

    // otevřít / zavřít UI dle hráče v triggeru
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            uiRoot.SetActive(true);
            ItemButton.isDragEnabled = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            uiRoot.SetActive(false);
            ItemButton.isDragEnabled = false;
            ResetStation();
        }
    }

    // volá InputSlotHandler, když do něj hráč přetáhne ShitPack z inventáře
    public void OnShitPackReceived(ItemButton btn)
    {
        hasPackInInput = true;
        // ikonku nech v inputSlotu – InputSlotHandler ji automaticky rodičovství změní
    }

    // volá WorkAreaDropHandler, když hráč přetáhne z inputSlotu na pracovní plochu
    public void OnWorkAreaDrop(GameObject go)
    {
        hasPackOnWorkArea = true;
        unwrapButton.interactable = true;
    }

    // kliknutí na "Rozbalit balíček"
    void OnClickUnwrap()
    {
        unwrapButton.interactable = false;

        // tady později spustíme animaci...
        // ale pro UI fázi právě jen přesuň ikonu do outputSlot

        // 1) odstran inputSlot obsah
        if (hasPackInInput)
        {
            inputSlot.Clear();
            hasPackInInput = false;
        }

        // 2) výsledek do outputSlot – vytvoř instanci Compost ikony
        var obj = Instantiate(compostPrefab);
        // outputSlot je klasický Slot, takže stačí drag&drop – ale pro UI fázi:
        outputSlot.OnDirectAdd(obj, 1);

        hasPackOnWorkArea = false;
    }

    // reset stavu (když hráč UI zavře)
    void ResetStation()
    {
        hasPackInInput = hasPackOnWorkArea = false;
        unwrapButton.interactable = false;
        inputSlot.Clear();
        // outputSlot nechte, hráč si to může vzít ven
    }
}