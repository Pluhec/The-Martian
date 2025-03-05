using System.Collections.Generic;
using UnityEngine;

public class TestInterakce : MonoBehaviour
{
    private void Start()
    {
        InteractableObject interactable = GetComponent<InteractableObject>();
        // Nastavujeme akce: první bude použita i jako Quick Action.
        interactable.SetActions(new List<string> { "Nasednout", "Opravit", "Zamknout" });
        Debug.Log("Akce nastaveny.");
    }
}