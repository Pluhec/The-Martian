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

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ExitMiniGame);
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