using UnityEngine;

public class Item1 : InteractableObject
{
    public string itemName = "Neznámý předmět";

    void Awake()
    {
        actions.Add("vycistit");
        actions.Add("nasednout");
        actions.Add("opravit");
    }

    public override void PerformAction(string action)
    {
        if (action == "vycistit")
        {
            Debug.Log("Vyčištěno");
        }
        
        if (action == "nasednout")
        {
            Debug.Log("Nasednuto!");
        }
        
        if (action == "opravit")
        {
            Debug.Log("Opraveno!!");
        }
    }
}