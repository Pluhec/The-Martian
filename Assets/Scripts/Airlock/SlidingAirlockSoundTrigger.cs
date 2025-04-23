using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class SlidingAirlockDoorTrigger : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    
    private bool isPlaying = false;
    private bool exitRequested = false;
    private float clipLength;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            exitRequested = false;
            TryPlayDoorSound();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPlaying)
            {
                TryPlayDoorSound();
            }
            else
            {
                exitRequested = true;
            }
        }
    }

    private void TryPlayDoorSound()
    {
        if (!isPlaying)
            StartCoroutine(PlayDoorSoundCoroutine());
    }

    private IEnumerator PlayDoorSoundCoroutine()
    {
        isPlaying = true;
        audioManager.PlaySlidingAirlockDoorSound();
        yield return new WaitForSeconds(clipLength);
        isPlaying = false;

        if (exitRequested)
        {
            exitRequested = false;
            StartCoroutine(PlayDoorSoundCoroutine());
        }
    }
}