using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    public OrderManager orderManager;
    public Movement playerMovement;
    public GameObject waterGameCanvas;
    public Button backButton;

    public ItemType[] targetSequence = new ItemType[]{
        ItemType.Pipe, ItemType.Pipe, ItemType.Catalyst, ItemType.Firewood
    };
    
    private GameObject toastPrefab;
    private Transform notificationsParent;

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ExitMiniGame);
            
        // Inicializace notifikačního systému
        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }
        else
        {
            Debug.LogWarning("Notification canvas s tagem 'NotificationSystem' nebyl nalezen.");
        }
    }

    public void OnIgniteButton() {
        var current = orderManager.currentOrder;

        for (int i = 0; i < targetSequence.Length; i++) {
            if (current[i].itemType != targetSequence[i]) {
                Debug.Log("Špatně: sekvence není správná.");
                TriggerPlayerDeath();
                return;
            }
        }

        Debug.Log("Správně: vyrobena voda.");
        
        // Zobrazení notifikace o úspěchu
        if (toastPrefab != null && notificationsParent != null)
        {
            var go = Instantiate(toastPrefab, notificationsParent);
            go.GetComponent<Toast>().Show("success", "Water is being produced!");
        }
        
        ExitMiniGame();
    }

    public void ExitMiniGame()
    {
        if (waterGameCanvas != null)
            waterGameCanvas.SetActive(false);
    }

    void TriggerPlayerDeath()
    {
        if (playerMovement != null)
        {
            playerMovement.oxygenDepletionTimer = 0f;
            playerMovement.Oxygen = 0f;
        }

        if (waterGameCanvas != null)
            waterGameCanvas.SetActive(false);
    }
}