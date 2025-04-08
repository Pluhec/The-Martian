using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public bool[] isFull;
    public GameObject[] slots;
    public Canvas inventoryCanvas; // Odkaz na Canvas inventáře

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(inventoryCanvas.gameObject); // Persistence Canvasu
            Debug.Log("Inventory initialized");
        }
        else
        {
            Debug.Log("Duplicate Inventory destroyed");
            Destroy(inventoryCanvas.gameObject);
            Destroy(gameObject);
        }
    }
}