using UnityEngine;

public class WaterGameInteraction : MonoBehaviour
{
    public GameObject waterGameCanvas;
    private bool playerInRange = false;
    
    void Start()
    {
        if (waterGameCanvas != null)
            waterGameCanvas.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowCanvas();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            HideCanvas();
        }
    }

    private void ShowCanvas()
    {
        if (waterGameCanvas != null)
        {
            waterGameCanvas.SetActive(true);
        }
    }
    
    private void HideCanvas()
    {
        if (waterGameCanvas != null)
        {
            waterGameCanvas.SetActive(false);
        }
    }
}