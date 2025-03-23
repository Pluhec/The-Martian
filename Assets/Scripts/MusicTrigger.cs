using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.PlayMenuMusic(audioManager.mainMenuMusic);
    }
}