using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StorageContainer : MonoBehaviour
{
    public GameObject uiRoot;
    public GameObject[] slots;
    public bool[]       isFull;

    void Awake()
    {
        if (slots == null || isFull == null || slots.Length != isFull.Length)
            Debug.LogError("[StorageContainer] slots / isFull délka nesouhlasí");
    }

    void Start() => Close();
    public void Open()  => uiRoot?.SetActive(true);
    public void Close() => uiRoot?.SetActive(false);

    /* ─── veřejné API ─── */

    public bool AddItemAt(int start, GameObject obj, int size)
    {
        CleanOrphans();
        if (start < 0 || start + size > slots.Length) return false;

        for (int j = 0; j < size; j++)
            if (isFull[start + j] || slots[start + j] == null)
                return AddItem(obj, size);

        return InsertItem(start, obj, size);
    }

    public bool AddItem(GameObject obj, int size)
    {
        CleanOrphans();
        for (int i = 0; i <= slots.Length - size; i++)
        {
            bool free = true;
            for (int j = 0; j < size; j++)
                if (isFull[i + j] || slots[i + j] == null) { free = false; break; }
            if (!free) continue;

            return InsertItem(i, obj, size);
        }
        return false;
    }

    public void RemoveItem(int main, int size) => VacateSlots(main, size, null);

    /* ─── interní ─── */

    void CleanOrphans()
    {
        for (int k = 0; k < slots.Length; k++)
        {
            var tf = slots[k]?.transform;
            if (tf == null) continue;

            if (tf.childCount == 0) { isFull[k] = false; continue; }

            var ch = tf.GetChild(0);
            bool hasBtn = ch.GetComponent<ItemButton>()      != null;
            bool hasPh  = ch.GetComponent<ItemPlaceholder>() != null;

            if (!hasBtn && !hasPh)
            {
                Destroy(ch.gameObject);
                isFull[k] = false;
            }
        }
    }

    bool InsertItem(int start, GameObject obj, int size)
    {
        var srcBtn = obj.GetComponent<ItemButton>();
        if (srcBtn == null || ContainsID(srcBtn.itemID)) return false;

        GameObject main; ItemButton btn;
        if (obj.scene.IsValid())
        {
            main = obj;
            main.transform.SetParent(slots[start].transform, false);
            btn  = main.GetComponent<ItemButton>();
        }
        else
        {
            main = Instantiate(obj, slots[start].transform, false);
            btn  = main.GetComponent<ItemButton>();
        }
        AlignInSlot(main);

        btn.Initialize(start, size, null, this);
        isFull[start] = true;

        for (int j = 1; j < size; j++)
        {
            int idx = start + j;
            var ph = Instantiate(obj, slots[idx].transform, false);
            AlignInSlot(ph);

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

            ph.AddComponent<ItemPlaceholder>().mainSlotIndex = start;
            isFull[idx] = true;
        }
        return true;
    }

    public void VacateSlots(int main, int size, GameObject keep)
    {
        for (int j = 0; j < size; j++)
        {
            int idx = main + j;
            if (idx >= slots.Length) continue;

            var tf = slots[idx]?.transform;
            if (tf != null && tf.childCount > 0)
            {
                var g  = tf.GetChild(0).gameObject;
                var ph = g.GetComponent<ItemPlaceholder>();
                if (ph != null && ph.mainSlotIndex != main) continue;
                if (g != keep) Destroy(g);
            }
            if (idx < isFull.Length) isFull[idx] = false;
        }
        AlignItems();
    }

    /* ─── zarovnání ─── */

    public void AlignItems()
    {
        int dst = 0, i = 0;
        while (i < slots.Length)
        {
            var tf = slots[i]?.transform;
            if (tf != null && tf.childCount > 0)
            {
                var obj = tf.GetChild(0).gameObject;
                var btn = obj.GetComponent<ItemButton>();
                if (btn == null) { i++; continue; }

                int size = btn.slotSize;

                if (i != dst)
                {
                    obj.transform.SetParent(slots[dst].transform, false);
                    AlignInSlot(obj);
                    btn.mainSlotIndex = dst;
                }

                for (int j = 1; j < size; j++)
                {
                    int src = i + j, tgt = dst + j;
                    if (src < slots.Length && slots[src]?.transform.childCount > 0)
                    {
                        var ph = slots[src].transform.GetChild(0).gameObject;
                        ph.transform.SetParent(slots[tgt].transform, false);
                        AlignInSlot(ph);
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

    /* ─── drobnosti ─── */

    bool ContainsID(string id)
    {
        foreach (var s in slots)
        {
            if (s == null || s.transform.childCount == 0) continue;
            var b = s.transform.GetChild(0).GetComponent<ItemButton>();
            if (b != null && b.itemID == id) return true;
        }
        return false;
    }

    void AlignInSlot(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f,0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation    = Quaternion.identity;
            rt.localScale       = Vector3.one;
        }
        else
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale    = Vector3.one;
        }
    }
    
    public SaveLoadManager.ContainerData ExportContainer()
    {
        var data = new SaveLoadManager.ContainerData
        {
            containerID = GetComponent<PersistentItem>()?.itemID
        };
        if (string.IsNullOrEmpty(data.containerID)) return data;

        int i = 0;
        while (i < slots.Length)
        {
            var tf = slots[i].transform;
            if (tf.childCount > 0)
            {
                var btn = tf.GetChild(0).GetComponent<ItemButton>();
                if (btn != null)
                {
                    data.items.Add(new SaveLoadManager.ItemData
                    {
                        itemID   = btn.itemID,
                        slotSize = btn.slotSize
                    });
                    i += btn.slotSize;
                    continue;
                }
            }
            i++;
        }
        return data;
    }

    public void ImportContainer(List<SaveLoadManager.ContainerData> list)
    {
        var id = GetComponent<PersistentItem>()?.itemID;
        if (string.IsNullOrEmpty(id)) return;

        var found = list.Find(c => c.containerID == id);
        if (found == null) return;

        /* vyčistit */
        for (int k = 0; k < slots.Length; k++)
        {
            if (slots[k].transform.childCount > 0)
                Destroy(slots[k].transform.GetChild(0).gameObject);
            isFull[k] = false;
        }

        foreach (var it in found.items)
        {
            GameObject prefab = /* lookup podle it.itemID */ null;
            if (prefab == null) continue;
            AddItem(prefab, it.slotSize);
        }
    }

}
