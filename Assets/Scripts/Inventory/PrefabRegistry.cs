using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry Instance { get; private set; }

    public string resourcesPath = "Items";

    private Dictionary<string, GameObject> map;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildRegistry()
    {
        if (map != null && map.Count > 0) return;
        map = new Dictionary<string, GameObject>();

        var prefabs = Resources.LoadAll<GameObject>(resourcesPath);
        foreach (var p in prefabs)
        {
            var def = p.GetComponent<ItemDefinition>();
            if (def != null && !string.IsNullOrEmpty(def.itemID))
                map[def.itemID] = p;
        }

        Debug.Log($"[PrefabRegistry] Loaded {map.Count} items from Resources/{resourcesPath}", this);
    }

    public GameObject Get(string id)
    {
        BuildRegistry();

        if (map.TryGetValue(id, out var prefab))
            return prefab;

        Debug.LogWarning($"[PrefabRegistry] Prefab with ID '{id}' not found!", this);
        return null;
    }
}