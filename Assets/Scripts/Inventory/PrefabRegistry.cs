using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry Instance { get; private set; }

    [Tooltip("Cesta k item prefabs ve Resources, bez 'Resources/' prefixu")]
    public string resourcesPath = "Items";

    // interní mapování itemID → prefab
    private Dictionary<string, GameObject> map;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // neinitializujeme map hned tady – BuildRegistry() to udělá lazy
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // metoda, která (z)buduje mapu pouze jednou
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

    // vrací prefab, nebo null + varování
    public GameObject Get(string id)
    {
        // ujistíme se, že máme mapu postavenou
        BuildRegistry();

        if (map.TryGetValue(id, out var prefab))
            return prefab;

        Debug.LogWarning($"[PrefabRegistry] Prefab with ID '{id}' not found!", this);
        return null;
    }
}