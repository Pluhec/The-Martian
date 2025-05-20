using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource radialMenu;
    [SerializeField] private AudioSource solarPanel;
    [SerializeField] private AudioSource solarPanelDone;
    [SerializeField] private AudioSource decompressionSource;
    [SerializeField] private AudioSource SlidingAirlockDoorSource;
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource menuButtons;
    [SerializeField] private AudioSource PoopPack;

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
    
    [Header("------ MainMenuMusic ------")]
    public AudioClip mainMenuMusic;
    
    [Header("------ MainMenuMusic ------")]
    public AudioClip hoverButton;
    public AudioClip clickButton;
    
    [Header("------ FertilizerStation ------")]
    public AudioClip PoopPackOpenning;

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

    public void PlayMenuMusic(AudioClip clip)
    {
        menuMusic.clip = clip;
        menuMusic.loop = true;
        menuMusic.Play();
    }
    
    public void PlayHoverButtonSound()
    {
        menuButtons.PlayOneShot(hoverButton);
    }

    public void PlayClickButtonSound()
    {
        menuButtons.PlayOneShot(clickButton);
    }
    
    public void PlayPoopPackOpenning()
    {
        menuButtons.PlayOneShot(PoopPackOpenning);
    }
}