using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public GameObject[] slots;      // UI sloty v Inspectoru
    public bool[]       isFull;     // stejná délka jako slots

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    /*──────────────────────────────────────────────────────────────*/
    /*  PUBLIC API                                                  */
    /*──────────────────────────────────────────────────────────────*/

    /// Přidá nebo přesune item přesně na slot startIndex (drop-slot).
    public bool AddItemAt(int startIndex, GameObject itemObj, int slotSize)
    {
        CleanOrphans();                                             // oprava zbytků
        if (startIndex < 0 || startIndex + slotSize > slots.Length) return false;

        for (int j = 0; j < slotSize; j++)
            if (isFull[startIndex + j])                             // blok obsazen
                return AddItem(itemObj, slotSize);                  // fallback

        return InsertItem(startIndex, itemObj, slotSize);
    }

    /// Najde první volný blok vlevo → vloží / přesune sem.
    public bool AddItem(GameObject itemObj, int slotSize)
    {
        CleanOrphans();
        for (int i = 0; i <= slots.Length - slotSize; i++)
        {
            bool free = true;
            for (int j = 0; j < slotSize; j++)
                if (isFull[i + j]) { free = false; break; }
            if (!free) continue;

            return InsertItem(i, itemObj, slotSize);
        }
        return false;
    }

    /// Smaže celý předmět (hlavní i placeholdery).
    public void RemoveItem(int mainIndex, int slotSize) =>
        VacateSlots(mainIndex, slotSize, null);

    /*──────────────────────────────────────────────────────────────*/
    /*  PRIVATE HELPERS                                             */
    /*──────────────────────────────────────────────────────────────*/

    /// Odstraní “sirotčí” placeholdery, které nemají ItemButton.
    void CleanOrphans()
    {
        for (int k = 0; k < slots.Length; k++)
        {
            var tf = slots[k].transform;
            if (tf.childCount == 0) { isFull[k] = false; continue; }

            if (tf.GetChild(0).GetComponent<ItemButton>() == null)
            {
                Destroy(tf.GetChild(0).gameObject);
                isFull[k] = false;
            }
        }
    }

    /// Vloží item na pozici startIndex – bez kontrol (volá jej AddItem/At).
    bool InsertItem(int startIndex, GameObject itemObj, int slotSize)
    {
        var srcBtn = itemObj.GetComponent<ItemButton>();
        if (srcBtn == null || ContainsID(srcBtn.itemID)) return false;

        /* hlavní ItemButton */
        GameObject main;
        ItemButton btn;
        if (itemObj.scene.IsValid())                         // přesun ve scéně
        {
            main = itemObj;
            main.transform.SetParent(slots[startIndex].transform, false);
            btn  = main.GetComponent<ItemButton>();
        }
        else                                                 // prefab pickup
        {
            main = Instantiate(itemObj, slots[startIndex].transform, false);
            btn  = main.GetComponent<ItemButton>();
        }

        btn.Initialize(startIndex, slotSize, this, null);
        isFull[startIndex] = true;

        /* placeholdery */
        for (int j = 1; j < slotSize; j++)
        {
            int idx = startIndex + j;
            var ph = Instantiate(itemObj, slots[idx].transform, false);

            Destroy(ph.GetComponent<ItemButton>());
            Destroy(ph.GetComponent<Button>());

            var img = ph.GetComponent<Image>();
            if (img != null)
            {
                img.color         = new Color(1,1,1,0.35f);
                img.raycastTarget = false;
            }
            var cg = ph.GetComponent<CanvasGroup>() ?? ph.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            ph.AddComponent<ItemPlaceholder>().mainSlotIndex = startIndex;
            isFull[idx] = true;
        }
        return true;
    }

    /// Uvolní blok slotů; zachová zadaný GameObject (hlavní ikonu v novém rodiči).
    public void VacateSlots(int mainIndex, int slotSize, GameObject keep)
    {
        for (int j = 0; j < slotSize; j++)
        {
            int idx = mainIndex + j;
            if (idx >= slots.Length) continue;

            var tf = slots[idx].transform;
            if (tf.childCount > 0)
            {
                var g = tf.GetChild(0).gameObject;

                // je to placeholder jiného itemu? → nechat být
                var ph = g.GetComponent<ItemPlaceholder>();
                if (ph != null && ph.mainSlotIndex != mainIndex) continue;

                if (g != keep) Destroy(g);
            }
            isFull[idx] = false;
        }
        AlignItems();
    }


    /// Zarovná ikony “doleva”.
    public void AlignItems()
    {
        int dst = 0, i = 0;
        while (i < slots.Length)
        {
            var tf = slots[i].transform;
            if (tf.childCount > 0)
            {
                var obj = tf.GetChild(0).gameObject;
                var btn = obj.GetComponent<ItemButton>();
                if (btn == null) { i++; continue; }

                int size = btn.slotSize;

                if (i != dst)
                {
                    obj.transform.SetParent(slots[dst].transform, false);
                    btn.mainSlotIndex = dst;
                }

                for (int j = 1; j < size; j++)
                {
                    int src = i + j, tgt = dst + j;
                    if (slots[src].transform.childCount > 0)
                    {
                        var ph = slots[src].transform.GetChild(0).gameObject;
                        ph.transform.SetParent(slots[tgt].transform, false);
                        var phc = ph.GetComponent<ItemPlaceholder>();
                        if (phc != null) phc.mainSlotIndex = dst;
                    }
                }

                for (int j = 0; j < size; j++) isFull[dst + j] = true;

                i += size; dst += size; continue;
            }
            i++;
        }
        for (int k = dst; k < isFull.Length; k++) isFull[k] = false;
    }

    bool ContainsID(string id)
    {
        foreach (var s in slots)
        {
            if (s.transform.childCount == 0) continue;
            var b = s.transform.GetChild(0).GetComponent<ItemButton>();
            if (b != null && b.itemID == id) return true;
        }
        return false;
    }
}
