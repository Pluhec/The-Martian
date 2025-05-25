using UnityEngine;

public class PersistentItem : MonoBehaviour
{
    public string itemID;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = gameObject.name + "_" + transform.position.ToString("F3");
        }
    }

    void Start()
    {
        if (PersistentDataManager.Instance != null && PersistentDataManager.Instance.IsItemCollected(itemID))
        {
            Destroy(gameObject);
        }
    }

    public void MarkCollected()
    {
        if (PersistentDataManager.Instance != null)
        {
            PersistentDataManager.Instance.MarkItemCollected(itemID);
        }
    }
}