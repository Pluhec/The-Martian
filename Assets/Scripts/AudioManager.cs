using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource radialMenu;
    
    
    [Header("------ RadialMenu ------")]
    public AudioClip openCloseMenu;
    public AudioClip howerAction;
    public AudioClip doAction;
    
    [Header("------ CleaningSolarPanel ------")]
    public AudioClip AirCleaningSound;
    
    public void PlayRadialMenu(AudioClip clip)
    {
        radialMenu.PlayOneShot(clip);
    }
    
    public void PlayCleaningSolarPanel(AudioClip clip)
    {
        radialMenu.clip = clip;
        radialMenu.loop = true;
        radialMenu.Play();
    }

    public void StopCleaningSolarPanel()
    {
        radialMenu.loop = false;
        radialMenu.Stop();
    }
}
