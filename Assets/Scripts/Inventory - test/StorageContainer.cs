using UnityEngine;
using UnityEngine.UI;

public class StorageContainer : MonoBehaviour
{
    public GameObject[] slots;
    public bool[] isFull;
    public GameObject canvas;

    public void Open() => canvas.SetActive(true);
    public void Close() => canvas.SetActive(false);

    public bool AddItem(GameObject itemButton, int slotSize)
    {
        for (int i = 0; i <= slots.Length - slotSize; i++)
        {
            bool free = true;
            for (int j = 0; j < slotSize; j++)
            {
                if (isFull[i + j]) { free = false; break; }
            }

            if (!free) continue;

            // Přesun hlavního ItemButtonu do slotu
            itemButton.transform.SetParent(slots[i].transform, false);
            var button = itemButton.GetComponent<ItemButton>();
            button.mainSlotIndex = i;
            button.slotSize = slotSize;
            //button.inventory = null;
            //button.storageContainer = this;

            isFull[i] = true;

            // Přesun nebo vytvoření placeholderů
            for (int j = 1; j < slotSize; j++)
            {
                int t = i + j;

                GameObject ph = new GameObject("Placeholder");
                ph.transform.SetParent(slots[t].transform, false);
                var img = ph.AddComponent<Image>();
                img.color = new Color(1, 1, 1, 0.35f);

                var comp = ph.AddComponent<ItemPlaceholder>();
                comp.mainSlotIndex = i;

                isFull[t] = true;
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(int mainIndex, int slotSize)
    {
        for (int j = 0; j < slotSize; j++)
        {
            int index = mainIndex + j;
            if (index < slots.Length)
            {
                Transform s = slots[index].transform;
                if (s.childCount > 0)
                {
                    Destroy(s.GetChild(0).gameObject);
                }

                isFull[index] = false;
            }
        }

        AlignItems();
    }

    public void AlignItems()
    {
        int dst = 0;
        int i = 0;

        while (i < slots.Length)
        {
            Transform slot = slots[i].transform;
            if (slot.childCount > 0)
            {
                GameObject obj = slot.GetChild(0).gameObject;
                var button = obj.GetComponent<ItemButton>();
                if (button == null)
                {
                    i++;
                    continue;
                }

                int size = button.slotSize;

                if (i != dst)
                {
                    obj.transform.SetParent(slots[dst].transform, false);
                    button.mainSlotIndex = dst;
                }

                for (int j = 1; j < size; j++)
                {
                    int src = i + j;
                    int tgt = dst + j;

                    if (slots[src].transform.childCount > 0)
                    {
                        GameObject ph = slots[src].transform.GetChild(0).gameObject;
                        ph.transform.SetParent(slots[tgt].transform, false);
                        var phComp = ph.GetComponent<ItemPlaceholder>();
                        if (phComp != null) phComp.mainSlotIndex = dst;
                    }
                }

                for (int j = 0; j < size; j++)
                    isFull[dst + j] = true;

                i += size;
                dst += size;
            }
            else
            {
                i++;
            }
        }

        for (int k = dst; k < isFull.Length; k++) isFull[k] = false;
    }
}