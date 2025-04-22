using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("UI Sloty Inventáře")]
    public GameObject[] slots;
    public bool[] isFull;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool AddItem(GameObject itemButtonPrefab, int slotSize)
    {
        for (int i = 0; i <= slots.Length - slotSize; i++)
        {
            bool free = true;
            for (int j = 0; j < slotSize; j++)
                if (isFull[i + j]) { free = false; break; }
            if (!free) continue;

            // Hlavní tlačítko
            GameObject mainItem = Instantiate(itemButtonPrefab, slots[i].transform, false);
            isFull[i] = true;

            var btn = mainItem.GetComponent<ItemButton>();
            btn.mainSlotIndex    = i;
            btn.slotSize         = slotSize;
            btn.inventory        = this;
            btn.storageContainer = null;

            // Placeholdery
            for (int j = 1; j < slotSize; j++)
            {
                int idx = i + j;
                GameObject placeholder = Instantiate(itemButtonPrefab, slots[idx].transform, false);

                Destroy(placeholder.GetComponent<ItemButton>());
                Destroy(placeholder.GetComponent<Button>());

                var img = placeholder.GetComponent<Image>();
                if (img != null) img.color = new Color(1, 1, 1, 0.35f);

                placeholder.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                isFull[idx] = true;
            }

            return true;
        }

        return false;
    }

        
    public void RemoveItem(int mainIndex, int slotSize)
    {
        for (int j = 0; j < slotSize; j++)
        {
            int idx = mainIndex + j;
            if (idx < slots.Length)
            {
                var slot = slots[idx].transform;
                if (slot.childCount > 0)
                    Destroy(slot.GetChild(0).gameObject);
                isFull[idx] = false;
            }
        }
        AlignItems();
    }

    public void AlignItems()
    {
        int dst = 0, i = 0;
        while (i < slots.Length)
        {
            var slot = slots[i].transform;
            if (slot.childCount > 0)
            {
                var obj = slot.GetChild(0).gameObject;
                var btn = obj.GetComponent<ItemButton>();
                if (btn == null) { i++; continue; }
                int size = btn.slotSize;

                // Přesun hlavního
                if (i != dst)
                {
                    obj.transform.SetParent(slots[dst].transform, false);
                    btn.mainSlotIndex = dst;
                }

                // Přesun placeholderů
                for (int j = 1; j < size; j++)
                {
                    int src = i + j, tgt = dst + j;
                    if (slots[src].transform.childCount > 0)
                    {
                        var ph = slots[src].transform.GetChild(0).gameObject;
                        ph.transform.SetParent(slots[tgt].transform, false);
                        var phComp = ph.GetComponent<ItemPlaceholder>();
                        if (phComp != null) phComp.mainSlotIndex = dst;
                    }
                }

                for (int j = 0; j < size; j++)
                    isFull[dst + j] = true;

                i += size; dst += size;
            }
            else i++;
        }
        for (int k = dst; k < isFull.Length; k++) isFull[k] = false;
    }
}