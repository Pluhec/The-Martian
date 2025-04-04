using UnityEngine;

public class Item1 : InteractableObject
{
    private Inventory inventory;
    public GameObject itemButton;

    void Awake()
    {
        actions.Add("sebrat");
        actions.Add("nasednout");
        actions.Add("opravit");
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public override void PerformAction(string action)
    {
        if (action == "sebrat")
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (inventory.isFull[i] == false)
                {
                    inventory.isFull[i] = true;
                    Instantiate(itemButton, inventory.slots[i].transform, false);
                    Destroy(gameObject);
                    break;
                }
            }
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