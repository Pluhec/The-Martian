using UnityEngine;

public class SlidingAirlockDoorSound : MonoBehaviour
{
    public AudioManager audioManager;

    public void PlaySoundOnDoorAnimation()
    {
        audioManager.PlaySlidingAirlockDoorSound();
    }

}
