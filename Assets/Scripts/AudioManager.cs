using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource radialMenu;
    [SerializeField] private AudioSource solarPanel;
    [SerializeField] private AudioSource solarPanelDone;
    [SerializeField] private AudioSource decompressionSource;
    [SerializeField] private AudioSource SlidingAirlockDoorSource;
    [SerializeField] private AudioSource sandSource;   
    [SerializeField] private AudioSource metalSource; 

    [Header("------ RadialMenu ------")]
    public AudioClip openCloseMenu;
    public AudioClip howerAction;
    public AudioClip doAction;

    [Header("------ CleaningSolarPanel ------")]
    public AudioClip AirCleaningSound;
    public AudioClip FullyCleanedSound;

    [Header("------ Decompression ------")]
    public AudioClip decompressionClip;

    [Header("------ SlidingAirlockDoor ------")]
    public AudioClip SlidingAirlockDoor;

    [Header("------ Footstep ------")]
    public AudioClip sandSoundClip;  
    public AudioClip metalSoundClip; 

    public void PlayRadialMenu(AudioClip clip)
    {
        radialMenu.PlayOneShot(clip);
    }
    
    public void PlayCleaningSolarPanel(AudioClip clip)
    {
        solarPanel.clip = clip;
        solarPanel.loop = true;
        solarPanel.Play();
    }

    public void StopCleaningSolarPanel()
    {
        solarPanel.loop = false;
        solarPanel.Stop();
    }
    
    public void PlayFullyCleanedSolarPanel(AudioClip clip)
    {
        solarPanelDone.PlayOneShot(clip);
    }
    
    public void PlayDecompressionSound()
    {
        decompressionSource.PlayOneShot(decompressionClip);
    }
    
    public void PlaySlidingAirlockDoorSound()
    {
        SlidingAirlockDoorSource.PlayOneShot(SlidingAirlockDoor);
    }
    
    public void PlaySandStep(float volume = 1f, float pitch = 1f)
    {
        sandSource.pitch = pitch;
        sandSource.PlayOneShot(sandSoundClip, volume);
    }
    
    public void PlayMetalStep(float volume = 1f, float pitch = 1f)
    {
        metalSource.pitch = pitch;
        metalSource.PlayOneShot(metalSoundClip, volume);
    }
}