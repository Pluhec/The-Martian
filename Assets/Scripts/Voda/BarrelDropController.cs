using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BarrelDropController : MonoBehaviour
{
    public BarrelRevealManager revealManager;

    void Start()
    {
        if (revealManager != null)
            revealManager.HideBarrel();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BarrelItem>() != null)
        {
            revealManager?.ShowBarrel();
            
            if (DroppedItemManager.Instance != null)
                DroppedItemManager.Instance.RemoveDroppedItem(other.gameObject);

            Destroy(other.gameObject);
        }
    }
}