using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroppedItemManager : MonoBehaviour
{
    public static DroppedItemManager Instance;

    [System.Serializable]
    public class DroppedItemData
    {
        public GameObject prefab;
        public Vector3 position;
        public string sceneName;
    }

    public List<DroppedItemData> droppedItems = new List<DroppedItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("DroppedItemManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddDroppedItem(GameObject prefab, Vector3 position)
    {
        DroppedItemData data = new DroppedItemData
        {
            prefab = prefab,
            position = position,
            sceneName = SceneManager.GetActiveScene().name
        };
        droppedItems.Add(data);
    }

    public void RemoveDroppedItem(GameObject item)
    {
        droppedItems.RemoveAll(d => d.position == item.transform.position && d.sceneName == SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var item in droppedItems)
        {
            if (item.sceneName != scene.name)
                continue;

            foreach (var pc in FindObjectsOfType<PathfinderComputer>())
            {
                if (pc.gameObject.name == item.prefab.name)
                    Destroy(pc.gameObject);
            }
        }

        foreach (var item in droppedItems)
        {
            if (item.sceneName != scene.name)
                continue;

            Instantiate(item.prefab, item.position, Quaternion.identity);
        }
    }

    public List<SaveLoadManager.DroppedData> ExportDropped()
    {
        var list = new List<SaveLoadManager.DroppedData>();
        foreach (var d in droppedItems)
        {
            var def = d.prefab.GetComponent<ItemDefinition>();
            if (def == null) continue;

            list.Add(new SaveLoadManager.DroppedData
            {
                prefabID = def.itemID,
                position = d.position,
                scene = d.sceneName
            });
        }
        return list;
    }

    public void ImportDropped(List<SaveLoadManager.DroppedData> list)
    {
        if (list == null) return;
        droppedItems.Clear();

        foreach (var d in list)
        {
            GameObject prefab = PrefabRegistry.Instance?.Get(d.prefabID);
            if (prefab == null) { Debug.LogWarning($"[Dropped] prefabKey \"{d.prefabID}\" nenalezen"); continue; }

            droppedItems.Add(new DroppedItemData
            {
                prefab = prefab,
                position = d.position,
                sceneName = d.scene
            });
        }
    }
}