using UnityEngine;

public class WaterGameInteraction : MonoBehaviour
{
    public GameObject waterGameCanvas;
    private bool playerInRange = false;
    
    void Start()
    {
        // Zajistíme, že canvas a nápověda jsou při startu skryté
        if (waterGameCanvas != null)
            waterGameCanvas.SetActive(false);
    }

    void Update()
    {
        // Pokud je hráč v dosahu a stiskne klávesu E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCanvas();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Předpokládám, že hráč má tag "Player"
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void ShowCanvas()
    {
        if (waterGameCanvas != null)
        {
            waterGameCanvas.SetActive(true);
        }
    }
}