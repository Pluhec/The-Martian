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
    [Tooltip("Reference to the StorageContainer component")] public StorageContainer container;
    [Tooltip("Items to spawn into the container")] public List<ContainerItemEntry> items = new List<ContainerItemEntry>();
    [Tooltip("Automatically respawn items periodically")] public bool autoSpawn = false;
    [Tooltip("Seconds between auto-spawns")] public float spawnInterval = 1f;
    [Tooltip("Spawn into random free slot")] public bool randomSlot = false;
    [Tooltip("Save and load container state")] public bool enablePersistence = false;

    private Coroutine spawnCoroutine;
    private string containerID;

    void OnValidate() {
        if (container == null)
            container = GetComponent<StorageContainer>();
    }

    void Awake() {
        OnValidate();
        if (container == null) {
            Debug.LogError("ContainerSpawner: No StorageContainer on " + gameObject.name);
            return;
        }
        var pid = container.GetComponent<PersistentItem>();
        containerID = (pid != null && !string.IsNullOrEmpty(pid.itemID)) ? pid.itemID : gameObject.name;

        bool loaded = enablePersistence && LoadPersistence();
        if (!loaded) {
            ClearContainer();
            FillContainer();
        }
    }

    void Start() {
        if (autoSpawn && spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(AutoSpawn());
    }

    void OnDisable() {
        if (spawnCoroutine != null) {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        if (enablePersistence)
            SavePersistence();
    }

    void ClearContainer() {
        if (container.slots == null) return;
        for (int i = 0; i < container.slots.Length; i++) {
            var slot = container.slots[i];
            if (slot && slot.transform.childCount > 0)
                DestroyImmediate(slot.transform.GetChild(0).gameObject);
            container.isFull[i] = false;
        }
        container.AlignItems();
    }

    public void FillContainer() {
        if (container.slots == null) return;
        var registry = PrefabRegistry.Instance;
        if (registry == null) return;

        foreach (var entry in items) {
            if (entry.count <= 0) continue;
            var prefab = registry.Get(entry.itemID);
            if (prefab == null) continue;

            for (int k = 0; k < entry.count; k++) {
                bool added = randomSlot
                    ? TryAddAtRandom(prefab, entry.slotSize)
                    : TryAddSequential(prefab, entry.slotSize);
                if (!added) {
                    Debug.LogWarning($"ContainerSpawner: No space for '{entry.itemID}' after {k} items.");
                    break;
                }
            }
        }
    }

    IEnumerator AutoSpawn() {
        while (autoSpawn) {
            if (container.slots == null) yield break;
            var registry = PrefabRegistry.Instance;
            foreach (var entry in items) {
                var prefab = registry.Get(entry.itemID);
                if (prefab == null) continue;
                bool added = randomSlot
                    ? TryAddAtRandom(prefab, entry.slotSize)
                    : TryAddSequential(prefab, entry.slotSize);
                if (!added)
                    Debug.LogWarning($"ContainerSpawner: AutoSpawn no space for '{entry.itemID}'.");
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool TryAddSequential(GameObject prefab, int size) {
        if (container.slots == null) return false;
        int maxStart = container.slots.Length - size;
        for (int i = 0; i <= maxStart; i++) {
            bool ok = true;
            for (int j = 0; j < size; j++) {
                if (container.isFull[i + j]) { ok = false; break; }
            }
            if (!ok) continue;
            if (container.AddItemAt(i, prefab, size))
                return true;
        }
        return false;
    }

    bool TryAddAtRandom(GameObject prefab, int size) {
        if (container.slots == null) return false;
        int maxStart = container.slots.Length - size;
        var valid = new List<int>();
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
        if (container.slots == null) return;
        var data = new PersistedContainerData { containerID = containerID };
        int i = 0;
        while (i < container.slots.Length) {
            var slot = container.slots[i];
            if (slot && slot.transform.childCount > 0) {
                var child = slot.transform.GetChild(0);
                var btn = child.GetComponent<ItemButton>();
                var def = child.GetComponent<ItemDefinition>();
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
        PlayerPrefs.SetString($"container_{containerID}", JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    bool LoadPersistence() {
        if (container.slots == null) return false;
        string key = $"container_{containerID}";
        if (!PlayerPrefs.HasKey(key)) return false;
        var data = JsonUtility.FromJson<PersistedContainerData>(PlayerPrefs.GetString(key));
        if (data == null || data.items.Count == 0) return false;
        for (int k = 0; k < container.slots.Length; k++) {
            var slot = container.slots[k];
            if (slot && slot.transform.childCount > 0)
                DestroyImmediate(slot.transform.GetChild(0).gameObject);
            container.isFull[k] = false;
        }
        foreach (var it in data.items) {
            var prefab = PrefabRegistry.Instance.Get(it.prefabKey);
            if (prefab == null) continue;
            var obj = Instantiate(prefab);
            var btn = obj.GetComponent<ItemButton>();
            if (btn != null) btn.itemID = it.uniqueID;
            container.AddItemAt(it.slotIndex, obj, it.slotSize);
        }
        return true;
    }
}
