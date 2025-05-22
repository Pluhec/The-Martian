using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public RectTransform slotsPanel;
    public GameObject slotPrefab;
    public List<RectTransform> slotTransforms;

    [Header("Item Prefabs")]
    public GameObject[] itemPrefabs;

    [HideInInspector]
    public List<DraggableItem> currentOrder = new List<DraggableItem>();

    void Start() {
        CreateSlots();
        SpawnItemsRandom();
    }

    void CreateSlots() {
        slotTransforms = new List<RectTransform>();
        float panelHeight = slotsPanel.rect.height;
        for (int i = 0; i < 4; i++) {
            GameObject s = Instantiate(slotPrefab, slotsPanel);
            RectTransform rt = s.GetComponent<RectTransform>();
            float y = panelHeight/2 - (i + 0.5f) * (panelHeight/4);
            rt.anchoredPosition = new Vector2(0, y);
            slotTransforms.Add(rt);
        }
    }

    void SpawnItemsRandom() {
        // Zamichani 
        GameObject[] shuffled = itemPrefabs.OrderBy(x => Random.value).ToArray();
        // Spawn
        for (int i = 0; i < shuffled.Length; i++) {
            GameObject go = Instantiate(shuffled[i], slotsPanel);
            var item = go.GetComponent<DraggableItem>();
    
            // nastaveni typu podle jmena
            if (shuffled[i].name.Contains("Pipe")) {
                item.itemType = ItemType.Pipe;
            } else if (shuffled[i].name.Contains("Catalyst")) {
                item.itemType = ItemType.Catalyst;
            } else if (shuffled[i].name.Contains("Firewood")) {
                item.itemType = ItemType.Firewood;
            }
    
            currentOrder.Add(item);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = slotTransforms[i].anchoredPosition;
            item.Init(this);
        }
    }

    public void InsertAt(DraggableItem item, int newIndex) {
        currentOrder.Remove(item);
        currentOrder.Insert(newIndex, item);
        AnimateAllToSlots();
    }

    public void AnimateAllToSlots() {
        for (int i = 0; i < currentOrder.Count; i++) {
            currentOrder[i].SnapTo(slotTransforms[i].anchoredPosition);
        }
    }
}