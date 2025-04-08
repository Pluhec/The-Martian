using UnityEngine;
using UnityEngine.UI;

public class Item1 : InteractableObject
{
    private Inventory inventory;
    public GameObject itemButton; // hlavní prefab
    public int slotSize = 2;      // velikost itemu ve slotech

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
            for (int i = 0; i <= inventory.slots.Length - slotSize; i++)
            {
                bool found = true;

                for (int j = 0; j < slotSize; j++)
                {
                    if (inventory.isFull[i + j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    // Vložit hlavní item button
                    GameObject mainItem = Instantiate(itemButton, inventory.slots[i].transform, false);
                    inventory.isFull[i] = true;

                    var itemScript = mainItem.GetComponent<ItemButton>();
                    itemScript.mainSlotIndex = i;
                    itemScript.slotSize = slotSize;

                    // Generování placeholderů jako kopie hlavního, ale upravené
                    for (int j = 1; j < slotSize; j++)
                    {
                        GameObject placeholder = Instantiate(itemButton, inventory.slots[i + j].transform, false);

                        // Zneškodnění interakcí
                        Destroy(placeholder.GetComponent<ItemButton>());
                        Destroy(placeholder.GetComponent<Button>()); // pokud tam je

                        // Průhlednost
                        Image img = placeholder.GetComponent<Image>();
                        if (img != null)
                        {
                            Color c = img.color;
                            c.a = 0.35f;
                            img.color = c;
                        }

                        // Označení jako placeholder
                        var p = placeholder.AddComponent<ItemPlaceholder>();
                        p.mainSlotIndex = i;

                        inventory.isFull[i + j] = true;
                    }

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
