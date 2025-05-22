using UnityEngine;

public class PotatoSoilSpot : InteractableObject
{
    [Header("Item IDs must match your inventory definitions")]    
    public string potatoItemID = "Potato";
    public string poopItemID   = "Poop";

    private Inventory inv;
    private GameObject toastPrefab;
    private Transform notificationsParent;

    void Awake()
    {
        // Inicializace inventáře
        inv = Inventory.Instance;

        // Nastavení toast notifikací
        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }

        // Definuj dostupné akce
        actions.Clear();
        actions.Add("Zasadit");
        actions.Add("Pohnojit");
    }

    public override void PerformAction(string action)
    {
        if (action == "Zasadit")
        {
            // Kontrola, zda má hráč bramboru
            if (!inv.ContainsID(potatoItemID))
            {
                ShowToast("warning", "Nemáš bramboru!");
                return;
            }
            // Kontrola, zda je stále místo pro sázení
            if (!PotatoFieldManager.Instance.CanPlant)
            {
                ShowToast("warning", "Nelze zasadit více brambor.");
                return;
            }
            // Odeber položku a zasad
            if (TryRemoveItem(potatoItemID))
            {
                PotatoFieldManager.Instance.PlantOne();
            }
        }
        else if (action == "Pohnojit")
        {
            // Kontrola, zda má hráč hnojivo
            if (!inv.ContainsID(poopItemID))
            {
                ShowToast("warning", "Nemáš hnojivo!");
                return;
            }
            // Kontrola, zda je co pohnojit
            if (!PotatoFieldManager.Instance.CanFertilize)
            {
                ShowToast("warning", "Nelze pohnojit další bramboru.");
                return;
            }
            // Odeber položku a pohnoj
            if (TryRemoveItem(poopItemID))
            {
                PotatoFieldManager.Instance.FertilizeOne();
            }
        }
        else
        {
            base.PerformAction(action);
        }
    }

    /// <summary>
    /// Najde a odebere z inventáře první položku s daným ID.
    /// </summary>
    private bool TryRemoveItem(string id)
    {
        for (int i = 0; i < inv.slots.Length; i++)
        {
            var slot = inv.slots[i].transform;
            if (slot.childCount == 0) continue;

            var child = slot.GetChild(0).gameObject;
            var btn = child.GetComponent<ItemButton>();
            var def = child.GetComponent<ItemDefinition>();
            if (btn != null && def != null && def.itemID == id)
            {
                inv.RemoveItem(btn.mainSlotIndex, btn.slotSize);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Zobrazí toast notifikaci.
    /// </summary>
    private void ShowToast(string type, string message)
    {
        if (toastPrefab != null && notificationsParent != null)
        {
            var toast = Instantiate(toastPrefab, notificationsParent);
            toast.GetComponent<Toast>()?.Show(type, message);
        }
    }
}
