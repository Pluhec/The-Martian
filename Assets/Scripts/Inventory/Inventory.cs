using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public GameObject[] slots;
    public bool[] isFull;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropFirstItem();
        }
    }

    public bool AddItemAt(int start, GameObject obj, int size)
    {
        CleanOrphans();
        if (start < 0 || start + size > slots.Length) return false;

        for (int j = 0; j < size; j++)
            if (isFull[start + j]) return AddItem(obj, size);

        return InsertItem(start, obj, size);
    }

    public bool AddItem(GameObject obj, int size)
    {
        CleanOrphans();
        for (int i = 0; i <= slots.Length - size; i++)
        {
            bool free = true;
            for (int j = 0; j < size; j++)
                if (isFull[i + j]) { free = false; break; }
            if (!free) continue;

            return InsertItem(i, obj, size);
        }
        return false;
    }

    public void RemoveItem(int main, int size) => VacateSlots(main, size, null);

    void CleanOrphans()
    {
        for (int k = 0; k < slots.Length; k++)
        {
            var tf = slots[k].transform;
            if (tf.childCount == 0) { isFull[k] = false; continue; }

            var ch = tf.GetChild(0);
            bool hasBtn = ch.GetComponent<ItemButton>() != null;
            bool hasPh = ch.GetComponent<ItemPlaceholder>() != null;

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
            btn = main.GetComponent<ItemButton>();
        }
        else
        {
            main = Instantiate(obj, slots[start].transform, false);
            btn = main.GetComponent<ItemButton>();
        }
        AlignInSlot(main);

        btn.Initialize(start, size, this, null);
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
                img.color = new Color(1,1,1,0.35f);
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
        Debug.Log($"[VacateSlots] called with main={main}, size={size}, keep={(keep!=null)}");

        for (int j = 0; j < size; j++)
        {
            int idx = main + j;
            if (idx >= slots.Length)
            {
                Debug.LogWarning($"[VacateSlots] idx {idx} out of range");
                continue;
            }

            var tf = slots[idx].transform;
            int beforeCount = tf.childCount;
            Debug.Log($"[VacateSlots] slot {idx} childCount before: {beforeCount}");

            if (tf.childCount > 0)
            {
                var g = tf.GetChild(0).gameObject;
                var ph = g.GetComponent<ItemPlaceholder>();
                if (ph != null && ph.mainSlotIndex != main)
                {
                    Debug.Log($"[VacateSlots] slot {idx} has placeholder for other item (mainSlotIndex={ph.mainSlotIndex}), skipping");
                    continue;
                }
                if (g != keep)
                {
                    Debug.Log($"[VacateSlots] Destroying GameObject '{g.name}' in slot {idx}");
                    Destroy(g);
                }
            }

            isFull[idx] = false;
            Debug.Log($"[VacateSlots] isFull[{idx}] set to false");
        }

        Debug.Log("[VacateSlots] calling AlignItems()");
        AlignItems();
    }

    public void DropFirstItem()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i].transform;
            if (slot.childCount == 0 || !isFull[i]) continue;

            var itemBtn = slot.GetChild(0).GetComponent<ItemButton>();
            if (itemBtn == null) continue;

            itemBtn.DropItem();
            break;
        }
    }

    public void AlignItems()
    {
        Debug.Log("[AlignItems] START");
        int dst = 0, i = 0;
        while (i < slots.Length)
        {
            var tf = slots[i].transform;
            if (tf.childCount > 0)
            {
                var obj = tf.GetChild(0).gameObject;
                var btn = obj.GetComponent<ItemButton>();
                if (btn != null)
                {
                    int size = btn.slotSize;
                    if (i != dst)
                        Debug.Log($"[AlignItems] Moving '{obj.name}' from slot {i} to slot {dst}");
                }
                i += (tf.childCount>0 && tf.GetChild(0).GetComponent<ItemButton>()!=null)
                    ? tf.GetChild(0).GetComponent<ItemButton>().slotSize
                    : 1;
                dst += tf.childCount>0 && tf.GetChild(0).GetComponent<ItemButton>()!=null
                    ? tf.GetChild(0).GetComponent<ItemButton>().slotSize
                    : 1;
                continue;
            }
            i++;
        }
        Debug.Log("[AlignItems] END");
    }

    public bool ContainsID(string id)
    {
        foreach (var s in slots)
        {
            if (s.transform.childCount == 0) continue;
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
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
        else
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }

    public List<SaveLoadManager.ItemData> ExportInventory()
    {
        var list = new List<SaveLoadManager.ItemData>();
        int i = 0;
        while (i < slots.Length)
        {
            var tf = slots[i].transform;
            if (tf.childCount > 0)
            {
                var btn = tf.GetChild(0).GetComponent<ItemButton>();
                var def = tf.GetChild(0).GetComponent<ItemDefinition>();
                if (btn != null && def != null)
                {
                    list.Add(new SaveLoadManager.ItemData
                    {
                        prefabKey = def.itemID,
                        uniqueID = btn.itemID,
                        slotSize = btn.slotSize
                    });
                    i += btn.slotSize;
                    continue;
                }
            }
            i++;
        }
        return list;
    }

    public void ImportInventory(List<SaveLoadManager.ItemData> list)
    {
        if (list == null || list.Count == 0) return;

        for (int k = 0; k < slots.Length; k++)
        {
            if (slots[k].transform.childCount > 0)
                Destroy(slots[k].transform.GetChild(0).gameObject);
            isFull[k] = false;
        }

        foreach (var it in list)
        {
            GameObject prefab = PrefabRegistry.Instance?.Get(it.prefabKey);
            if (prefab == null) { Debug.LogWarning($"[Inventory] prefabKey \"{it.prefabKey}\" nenalezen"); continue; }

            var obj = Instantiate(prefab);
            var btn = obj.GetComponent<ItemButton>();
            if (btn != null) btn.itemID = it.uniqueID;

            AddItem(obj, it.slotSize);
        }
    }
}