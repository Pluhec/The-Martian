using UnityEngine;

public class Item : InteractableObject
{
    public string itemName = "Neznámý předmět";

    void Awake()
    {
        actions.Add("Nasednout");
    }

    public override void PerformAction(string action)
    {
        if (action == "Nasednout")
        {
            Debug.Log($"Hráč sebral {itemName}!");
        }
    }
}