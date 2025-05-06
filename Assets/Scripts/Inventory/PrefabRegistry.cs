using System.Collections.Generic;
using UnityEngine;

public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry Instance { get; private set; }

    Dictionary<string, GameObject> map = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        BuildRegistry();
    }

#if UNITY_EDITOR
    // Když spustíš hru z Editoru, přidej i prefaby, které nejsou v Resources
    [UnityEditor.InitializeOnLoadMethod]
    static void RebuildInEditor()
    {
        if (Application.isPlaying) return;     // stačí v play-mode
        var reg = FindObjectOfType<PrefabRegistry>();
        if (reg != null) reg.BuildRegistry();
    }
#endif

    void BuildRegistry()
    {
        map.Clear();

        /* 1) ­--- načíst vše z adresáře Resources/Items/ --- */
        var prefabs = Resources.LoadAll<GameObject>("Items");
        foreach (var p in prefabs)
        {
            var def = p.GetComponent<ItemDefinition>();
            if (def != null && !map.ContainsKey(def.itemID))
                map.Add(def.itemID, p);
        }

#if UNITY_EDITOR
        /* 2) ­--- Editor: najít i prefaby, které NEjsou v Resources --- */
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab");
        foreach (var g in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
            var prefab  = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var def     = prefab != null ? prefab.GetComponent<ItemDefinition>() : null;
            if (def != null && !map.ContainsKey(def.itemID))
                map.Add(def.itemID, prefab);
        }
#endif
    }

    public GameObject Get(string id) =>
        map.TryGetValue(id, out var p) ? p : null;
}