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
            if (item.sceneName == scene.name)
            {
                Instantiate(item.prefab, item.position, Quaternion.identity);
            }
        }
    }
}