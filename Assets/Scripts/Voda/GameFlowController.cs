using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    public OrderManager orderManager;
    public Movement playerMovement;
    public GameObject waterGameCanvas;
    public Button backButton;

    public ItemType[] targetSequence = new ItemType[]{
        ItemType.Pipe, ItemType.Pipe, ItemType.Catalyst, ItemType.Firewood
    };
    
    public int questID;
    private QuestManager questManager;
    private QuestTablet questTablet;
    
    private GameObject toastPrefab;
    private Transform notificationsParent;

    void Start()
    {
        questManager = QuestManager.Instance;
        questTablet = FindObjectOfType<QuestTablet>();

        if (backButton != null)
            backButton.onClick.AddListener(ExitMiniGame);
            
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
        
        if (toastPrefab != null && notificationsParent != null)
        {
            var go = Instantiate(toastPrefab, notificationsParent);
            go.GetComponent<Toast>().Show("success", "Water is being produced!");
        }

        if (questManager != null)
        {
            questManager.MarkQuestAsCompletedByID(questID);
            if (questTablet != null)
                questTablet.UpdateQuestList();
            TimeManager.Instance.ResumeTime();
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
            playerMovement.OxygenTimeBeforeDeath = 2f;
            playerMovement.oxygenDepletionTimer = 2f;
            playerMovement.Oxygen = 0f;
            StartCoroutine(DelayedPlayerDeath());
        }

        if (waterGameCanvas != null)
            waterGameCanvas.SetActive(false);
    }

    IEnumerator DelayedPlayerDeath()
    {
        if (playerMovement.screenFade != null)
            playerMovement.screenFade.color = new Color(0, 0, 0, 1f);
        
        playerMovement.alive = false;
        playerMovement.ShowDeathMessage();
        
        yield return null;
    }
}