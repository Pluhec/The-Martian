using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BarrelDropController : MonoBehaviour
{
    [Tooltip("Sem přetáhni GameObject s BarrelRevealManagerem")]
    public BarrelRevealManager revealManager;

    void Start()
    {
        // volitelné: reset barrelu při startu
        if (revealManager != null)
            revealManager.HideBarrel();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // když hráč hodí BarrelItem na tuto zónu, objeví se barrel
        if (other.GetComponent<BarrelItem>() != null)
        {
            revealManager?.ShowBarrel();

            // aby se ten drop nevrátil po reloadu scény
            if (DroppedItemManager.Instance != null)
                DroppedItemManager.Instance.RemoveDroppedItem(other.gameObject);

            Destroy(other.gameObject);
        }
    }
}