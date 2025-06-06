using UnityEngine;
using UnityEngine.UI;

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
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) box?.Close();
    }
}
