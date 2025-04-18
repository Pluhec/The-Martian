using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    public List<string> actions = new List<string>();

    public List<string> GetActions()
    {
        return actions;
    }
 
    public virtual void PerformAction(string action)
    {
        Debug.Log($"Hráč provedl akci: {action} na objektu {gameObject.name}");
    }
}