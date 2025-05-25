using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    const string FILE = "savegame.json";

    string BaseDir =>
#if UNITY_EDITOR
        Path.Combine(Application.dataPath, "Saves");
#else
        Application.persistentDataPath;
#endif

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        Load();
    }

    void OnApplicationQuit() => Save();

    [System.Serializable] public class ItemData { public string prefabKey; public string uniqueID; public int slotSize; }
    [System.Serializable] public class ContainerData { public string containerID; public List<ItemData> items = new(); }
    [System.Serializable] public class DroppedData { public string prefabID; public Vector3 position; public string scene; }

    [System.Serializable] class SaveData
    {
        public List<ItemData> inventory = new();
        public List<ContainerData> containers = new();
        public List<DroppedData> dropped = new();
        public List<string> collected = new();
    }

    public void Save()
    {
        var data = new SaveData
        {
            inventory = Inventory.Instance.ExportInventory(),
            dropped = DroppedItemManager.Instance.ExportDropped(),
            collected = PersistentDataManager.Instance.ExportCollected()
        };

        foreach (var box in FindObjectsOfType<StorageContainer>(true))
            data.containers.Add(box.ExportContainer());

        if (!Directory.Exists(BaseDir)) Directory.CreateDirectory(BaseDir);
        File.WriteAllText(Path.Combine(BaseDir, FILE), JsonUtility.ToJson(data, true));
#if UNITY_EDITOR
        Debug.Log($"[SaveLoad] Uloženo: {BaseDir}/{FILE}");
#endif
    }

    public void Load()
    {
        string path = Path.Combine(BaseDir, FILE);
        if (!File.Exists(path)) return;

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        if (data == null) { Debug.LogWarning("[SaveLoad] Chybný JSON!"); return; }

        Inventory.Instance.ImportInventory(data.inventory);
        DroppedItemManager.Instance.ImportDropped(data.dropped);
        PersistentDataManager.Instance.ImportCollected(data.collected);

        StartCoroutine(DelayContainers(data.containers));
    }

    System.Collections.IEnumerator DelayContainers(List<ContainerData> list)
    {
        yield return null;
        foreach (var box in FindObjectsOfType<StorageContainer>(true))
            box.ImportContainer(list);
    }
}