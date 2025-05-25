using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ContainerRepository
{
    static readonly Dictionary<string, GameObject> map = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void Register(string id, GameObject box)
    {
        if (!map.ContainsKey(id)) map.Add(id, box);
        Debug.Log($"[ContainerRepository] Registrována bedna: {id}");
    }

    public static bool TryGet(string id, out GameObject box) =>
        map.TryGetValue(id, out box);

    public static void Unregister(string id)
    {
        if (map.ContainsKey(id))
        {
            map.Remove(id);
            Debug.Log($"[ContainerRepository] Odregistrována bedna: {id}");
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var container in map.Values)
        {
            if (container != null && container.activeSelf)
            {
                container.SetActive(false);
                Debug.Log($"[ContainerRepository] Deaktivována bedna při načtení scény: {scene.name}");
            }
        }
    }
}