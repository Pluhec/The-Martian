using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource radialMenu;
    [SerializeField] AudioSource solarPanel;
    [SerializeField] AudioSource solarPanelDone;
    [SerializeField] AudioSource decompressionSource;
    
    [Header("------ RadialMenu ------")]
    public AudioClip openCloseMenu;
    public AudioClip howerAction;
    public AudioClip doAction;
    
    [Header("------ CleaningSolarPanel ------")]
    public AudioClip AirCleaningSound;
    public AudioClip FullyCleanedSound;
    
    [Header("decompression")]
    public AudioClip decompressionClip;
    
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
}
