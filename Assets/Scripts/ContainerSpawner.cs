using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ContainerItemEntry {
    public string itemID;
    public int count = 1;
    public int slotSize = 1;
}

[System.Serializable]
class PersistedItemData {
    public string prefabKey;
    public string uniqueID;
    public int slotSize;
    public int slotIndex;
}

[System.Serializable]
class PersistedContainerData {
    public string containerID;
    public List<PersistedItemData> items = new List<PersistedItemData>();
}

public class ContainerSpawner : MonoBehaviour {
    public StorageContainer container;
    public List<ContainerItemEntry> items = new List<ContainerItemEntry>();
    public bool autoSpawn = false;
    public float spawnInterval = 1f;
    public bool randomSlot = false;
    public bool enablePersistence = false;

    private Coroutine spawnCoroutine;
    private string containerID;

    void OnValidate() {
        if (container == null)
            container = GetComponent<StorageContainer>();
    }

    void Awake() {
        if (container == null) {
            container = GetComponent<StorageContainer>();
            if (container == null) {
                Debug.LogError("ContainerSpawner: StorageContainer missing");
                return;
            }
        }
        // unique ID for persistence
        var pid = container.GetComponent<PersistentItem>();
        containerID = (pid != null && !string.IsNullOrEmpty(pid.itemID)) ? pid.itemID : gameObject.name;

        // initial load or fill
        bool loaded = false;
        if (enablePersistence)
            loaded = LoadPersistence();
        if (!loaded) {
            ClearContainer();
            FillContainer();
        }
    }

    void OnEnable() {
        // rebuild UI layout
        var rect = container.GetComponent<RectTransform>();
        if (rect != null) {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        // start auto-spawn
        if (autoSpawn && spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(AutoSpawn());
    }

    void OnDisable() {
        // stop auto-spawn
        if (spawnCoroutine != null) {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        // save state
        if (enablePersistence)
            SavePersistence();
    }

    void ClearContainer() {
        for (int i = 0; i < container.slots.Length; i++) {
            var tf = container.slots[i].transform;
            if (tf.childCount > 0)
                DestroyImmediate(tf.GetChild(0).gameObject);
            container.isFull[i] = false;
        }
        container.AlignItems();
    }

    public void FillContainer() {
        foreach (var entry in items) {
            GameObject prefab = PrefabRegistry.Instance.Get(entry.itemID);
            if (prefab == null) {
                Debug.LogWarning($"ContainerSpawner: Prefab '{entry.itemID}' not found.");
                continue;
            }
            for (int k = 0; k < entry.count; k++) {
                bool added = randomSlot
                    ? TryAddAtRandom(prefab, entry.slotSize)
                    : container.AddItem(prefab, entry.slotSize);
                if (!added) break;
            }
        }
    }

    IEnumerator AutoSpawn() {
        while (autoSpawn) {
            foreach (var entry in items) {
                GameObject prefab = PrefabRegistry.Instance.Get(entry.itemID);
                if (prefab == null) continue;
                if (randomSlot)
                    TryAddAtRandom(prefab, entry.slotSize);
                else
                    container.AddItem(prefab, entry.slotSize);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool TryAddAtRandom(GameObject prefab, int size) {
        int maxStart = container.slots.Length - size;
        List<int> valid = new List<int>();
        for (int i = 0; i <= maxStart; i++) {
            bool ok = true;
            for (int j = 0; j < size; j++) {
                if (container.isFull[i + j]) { ok = false; break; }
            }
            if (ok) valid.Add(i);
        }
        if (valid.Count == 0) return false;
        int idx = valid[Random.Range(0, valid.Count)];
        return container.AddItemAt(idx, prefab, size);
    }

    void SavePersistence() {
        var data = new PersistedContainerData { containerID = containerID };
        int i = 0;
        while (i < container.slots.Length) {
            var tf = container.slots[i].transform;
            if (tf.childCount > 0) {
                var btn = tf.GetChild(0).GetComponent<ItemButton>();
                var def = tf.GetChild(0).GetComponent<ItemDefinition>();
                if (btn != null && def != null) {
                    data.items.Add(new PersistedItemData {
                        prefabKey = def.itemID,
                        uniqueID = btn.itemID,
                        slotSize = btn.slotSize,
                        slotIndex = i
                    });
                    i += btn.slotSize;
                    continue;
                }
            }
            i++;
        }
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString($"container_{containerID}", json);
        PlayerPrefs.Save();
    }

    bool LoadPersistence() {
        var key = $"container_{containerID}";
        if (!PlayerPrefs.HasKey(key)) return false;
        var data = JsonUtility.FromJson<PersistedContainerData>(PlayerPrefs.GetString(key));
        if (data == null || data.items.Count == 0) return false;
        for (int k = 0; k < container.slots.Length; k++) {
            var tf = container.slots[k].transform;
            if (tf.childCount > 0)
                DestroyImmediate(tf.GetChild(0).gameObject);
            container.isFull[k] = false;
        }
        foreach (var it in data.items) {
            GameObject prefab = PrefabRegistry.Instance.Get(it.prefabKey);
            if (prefab == null) continue;
            var obj = Instantiate(prefab);
            var btn = obj.GetComponent<ItemButton>();
            if (btn != null) btn.itemID = it.uniqueID;
            container.AddItemAt(it.slotIndex, obj, it.slotSize);
        }
        return true;
    }
}
