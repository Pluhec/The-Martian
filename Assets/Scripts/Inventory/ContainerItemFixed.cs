using UnityEngine;
using UnityEngine.UI;

/// Jeden skript pro bednu – umí ji sebrat *a* otevřít / zavřít
/// (sebrat = přesun do DontDestroyOnLoad + ikona v inventáři).
public class ContainerItemFixed : InteractableObject
{
    StorageContainer box;
    Inventory        inventory;
    PersistentItem   pItem;

    void Awake()
    {
        box = GetComponent<StorageContainer>();
        inventory = Inventory.Instance;
        pItem = GetComponent<PersistentItem>();
        actions.Add("otevřít");
    }

    public override void PerformAction(string action)
    {
        switch (action)
        {
            case "otevřít":  box?.Open();  break;
        }
    }
}
