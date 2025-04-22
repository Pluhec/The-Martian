using UnityEngine;
using UnityEngine.UI;

public class StorageContainer : MonoBehaviour
{
    [Header("Root GameObject obsahující všechny sloty (např. Panel pod Canvasem)")]
    public GameObject uiRoot;

    [Header("Sloty kontejneru")]
    public GameObject[] slots;

    [Header("Musí mít stejnou délku jako slots[]")]
    public bool[] isFull;

    void Awake()
    {
        if (slots == null || isFull == null || slots.Length != isFull.Length)
        {
            Debug.LogError($"[StorageContainer] špatné nastavení: slots.Length={slots?.Length} isFull.Length={isFull?.Length}");
        }
    }

    void Start()
    {
        Close(); // na začátku skryj UI panel
    }

    public void Open()
    {
        if (uiRoot != null) uiRoot.SetActive(true);
    }

    public void Close()
    {
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    public bool AddItem(GameObject itemButtonPrefab, int slotSize)
    {
        if (slots == null || isFull == null) return false;

        for (int i = 0; i <= slots.Length - slotSize; i++)
        {
            // 1) Najdi první volný blok
            bool free = true;
            for (int j = 0; j < slotSize; j++)
                if (isFull[i + j]) { free = false; break; }
            if (!free) continue;

            // 2) Vytvoř hlavní ItemButton
            GameObject mainItem = Instantiate(itemButtonPrefab, slots[i].transform, false);
            AlignInSlot(mainItem);

            var btn = mainItem.GetComponent<ItemButton>();
            btn.Initialize(i, slotSize, /*inventory*/ null, /*container*/ this);
            isFull[i] = true;

            // 3) Vytvoř placeholdery
            for (int j = 1; j < slotSize; j++)
            {
                int idx = i + j;
                GameObject ph = Instantiate(itemButtonPrefab, slots[idx].transform, false);
                AlignInSlot(ph);

                // odstran komponentu ItemButton + Button
                Destroy(ph.GetComponent<ItemButton>());
                Destroy(ph.GetComponent<Button>());

                var img = ph.GetComponent<Image>();
                if (img != null) img.color = new Color(1, 1, 1, 0.35f);

                ph.AddComponent<ItemPlaceholder>().mainSlotIndex = i;
                isFull[idx] = true;
            }

            return true;
        }

        return false;
    }

    public void RemoveItem(int mainIndex, int slotSize)
    {
        if (slots == null || isFull == null) return;

        for (int j = 0; j < slotSize; j++)
        {
            int idx = mainIndex + j;
            if (idx < slots.Length)
            {
                Transform s = slots[idx].transform;
                if (s.childCount > 0) Destroy(s.GetChild(0).gameObject);
                isFull[idx] = false;
            }
        }

        AlignItems();
    }

    public void AlignItems()
    {
        if (slots == null || isFull == null) return;

        int dst = 0, i = 0;
        while (i < slots.Length)
        {
            var slotTf = slots[i].transform;
            if (slotTf.childCount > 0)
            {
                var obj = slotTf.GetChild(0).gameObject;
                var btn = obj.GetComponent<ItemButton>();
                if (btn != null)
                {
                    int size = btn.slotSize;

                    // přesun hlavního
                    if (i != dst)
                    {
                        obj.transform.SetParent(slots[dst].transform, false);
                        AlignInSlot(obj);
                        btn.mainSlotIndex = dst;
                    }

                    // přesun placeholderů
                    for (int j = 1; j < size; j++)
                    {
                        int src = i + j, tgt = dst + j;
                        if (src < slots.Length && slots[src].transform.childCount > 0)
                        {
                            var ph = slots[src].transform.GetChild(0).gameObject;
                            ph.transform.SetParent(slots[tgt].transform, false);
                            AlignInSlot(ph);
                            var phComp = ph.GetComponent<ItemPlaceholder>();
                            if (phComp != null) phComp.mainSlotIndex = dst;
                        }
                    }

                    // označ jako obsazené
                    for (int j = 0; j < size; j++)
                        isFull[dst + j] = true;

                    i += size;
                    dst += size;
                    continue;
                }
            }
            i++;
        }

        // vyčisti zbytek
        for (int k = dst; k < isFull.Length; k++)
            isFull[k] = false;
    }

    /// <summary>
    /// zarovná instanci do středu slotu
    /// </summary>
    private void AlignInSlot(GameObject go)
    {
        // pokud je to UI prvek
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            // pivot/anchor do středu a pozice nula
            rt.anchorMin       = new Vector2(0.5f, 0.5f);
            rt.anchorMax       = new Vector2(0.5f, 0.5f);
            rt.pivot           = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation   = Quaternion.identity;
            rt.localScale      = Vector3.one;
        }
        else
        {
            // pro non‑UI
            go.transform.localPosition  = Vector3.zero;
            go.transform.localRotation  = Quaternion.identity;
            go.transform.localScale     = Vector3.one;
        }
    }
}
