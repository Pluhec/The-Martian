using UnityEngine;

public class Item1 : InteractableObject
{
    public string itemName = "Neznámý předmět";

    void Awake()
    {
        actions.Add("Ahoj");
        actions.Add("nevim");
        actions.Add("nevim2");
    }

    public override void PerformAction(string action)
    {
        if (action == "Ahoj")
        {
            Debug.Log($"Ahoj Item1 {itemName}!");
        }
        
        if (action == "nevim")
        {
            Debug.Log($"nevim Item1 {itemName}!");
        }
    }
}