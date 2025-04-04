using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory Inventory;
    public int i;
    
    void Start()
    {
        Inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    
    void Update()
    {
        if (transform.childCount <= 0)
        {
            Inventory.isFull[i] = false;
        }
        {
            
        }
    }
    
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Spawn>().SpawnDroppedItem();
            GameObject.Destroy(child.gameObject);
        }
    }
}
