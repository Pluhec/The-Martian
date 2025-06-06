using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    private HashSet<string> collectedItems = new HashSet<string>();

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

    public void MarkItemCollected(string itemID)
    {
        collectedItems.Add(itemID);
    }

    public bool IsItemCollected(string itemID)
    {
        return collectedItems.Contains(itemID);
    }

    public List<string> ExportCollected() => new List<string>(collectedItems);

    public void ImportCollected(List<string> list)
    {
        if (list == null) return;
        collectedItems = new HashSet<string>(list);
    }
}