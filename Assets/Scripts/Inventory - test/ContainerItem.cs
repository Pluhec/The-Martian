using UnityEngine;

public class ContainerItem : InteractableObject
{
    private StorageContainer container;

    void Awake()
    {
        container = GetComponent<StorageContainer>();
        actions.Add("otevřít");
        actions.Add("zavřít");
    }

    public override void PerformAction(string action)
    {
        if (container == null) return;
        if (action == "otevřít") container.Open();
        else if (action == "zavřít") container.Close();
    }
}