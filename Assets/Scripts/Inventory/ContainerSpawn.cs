using UnityEngine;

public class ContainerSpawn : MonoBehaviour
{
    [HideInInspector] public string containerID;
    [HideInInspector] public ItemButton iconButton;

    public void SpawnContainer(Vector2 pos)
    {
        if (!ContainerRepository.TryGet(containerID, out var box))
        {
            Debug.LogWarning($"[ContainerSpawn] Box {containerID} v úschovně nenalezen.");
            return;
        }

        box.transform.position = pos;
        box.transform.rotation = Quaternion.identity;
        box.SetActive(true);
        ContainerRepository.Unregister(containerID);

        var inv = Inventory.Instance;
        if (inv != null && iconButton != null)
        {
            inv.RemoveItem(iconButton.mainSlotIndex, iconButton.slotSize);
        }
        else
        {
            Destroy(iconButton.gameObject);
        }
    }
}