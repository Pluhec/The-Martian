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
            GameObject.Destroy(child.gameObject);
        }
    }
}
