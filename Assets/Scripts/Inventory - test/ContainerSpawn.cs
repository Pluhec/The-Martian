using UnityEngine;

/// Nahrazuje běžný `Spawn` – bednu neinstancuje, jen ji přesune do scény.
public class ContainerSpawn : MonoBehaviour
{
    [HideInInspector] public string     containerID;
    [HideInInspector] public ItemButton iconButton;     // odkaz na vlastní ikonu

    public void SpawnContainer(Vector2 pos)
    {
        if (!ContainerRepository.TryGet(containerID, out var box))
        {
            Debug.LogWarning($"[ContainerSpawn] Box {containerID} v úschovně nenalezen.");
            return;
        }

        // 1) vrátíme bednu do aktuální scény
        box.transform.position = pos;
        box.transform.rotation = Quaternion.identity;
        box.SetActive(true);
        ContainerRepository.Unregister(containerID);

        // 2) odstraníme ikonu + placeholdery „oficiální“ cestou
        var inv = Inventory.Instance;
        if (inv != null && iconButton != null)
        {
            inv.RemoveItem(iconButton.mainSlotIndex, iconButton.slotSize);
        }
        else
        {
            // nouzově – kdyby něco selhalo
            Destroy(iconButton.gameObject);
        }
    }
}