using System.Collections.Generic;
using UnityEngine;

/// Úschovna deaktivovaných beden v DontDestroyOnLoad
public static class ContainerRepository
{
    static readonly Dictionary<string, GameObject> map = new();

    public static void Register(string id, GameObject box)
    {
        if (!map.ContainsKey(id)) map.Add(id, box);
    }

    public static bool TryGet(string id, out GameObject box) =>
        map.TryGetValue(id, out box);

    public static void Unregister(string id) => map.Remove(id);
}