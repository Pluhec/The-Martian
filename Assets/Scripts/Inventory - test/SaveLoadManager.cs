using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    const string FILE = "savegame.json";

    /*──────── cesta podle platformy ────────*/
    string BaseDir =>
#if UNITY_EDITOR
        Path.Combine(Application.dataPath, "Saves");      // zapisuje do Assets/Saves
#else
        Application.persistentDataPath;                   // build – bezpečné místo pro zápis
#endif

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        Load();
    }

    void OnApplicationQuit() => Save();

    /*──────── datové třídy ────────*/
    [System.Serializable] public class ItemData       { public string itemID;  public int slotSize; }
    [System.Serializable] public class ContainerData  { public string containerID; public List<ItemData> items = new(); }
    [System.Serializable] public class DroppedData    { public string prefabID;   public Vector3 position; public string scene; }

    [System.Serializable] class SaveData
    {
        public List<ItemData>     inventory  = new();
        public List<ContainerData> containers = new();
        public List<DroppedData>  dropped    = new();
    }

    /*──────── SAVE ────────*/
    public void Save()
    {
        var data = new SaveData
        {
            inventory  = Inventory.Instance.ExportInventory(),                 // NEW
            dropped    = DroppedItemManager.Instance.ExportDropped()           // NEW
        };

        foreach (var box in FindObjectsOfType<StorageContainer>(true))
            data.containers.Add(box.ExportContainer());                       // NEW

        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(BaseDir, FILE), json);
    }

    /*──────── LOAD ────────*/
    public void Load()
    {
        string path = Path.Combine(BaseDir, FILE);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);
        if (data == null) return;

        Inventory.Instance.ImportInventory(data.inventory);                    // NEW
        DroppedItemManager.Instance.ImportDropped(data.dropped);               // NEW
        StartCoroutine(DelayContainers(data.containers));
    }

    System.Collections.IEnumerator DelayContainers(List<ContainerData> list)
    {
        yield return null; // počkej jeden frame, ať se všechny bedny stihnou spawnout
        foreach (var box in FindObjectsOfType<StorageContainer>(true))
            box.ImportContainer(list);                                         // NEW
    }
}
