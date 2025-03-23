using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource radialMenu;
    [SerializeField] AudioSource menuMusic;
    
    [Header("------ RadialMenu ------")]
    public AudioClip openCloseMenu;
    public AudioClip howerAction;
    public AudioClip doAction;
    
    [Header("------ Music ------")]
    
    public AudioClip mainMenuMusic;
    
    public void PlayRadialMenu(AudioClip clip)
    {
        radialMenu.PlayOneShot(clip);
    }
    
    public void PlayMenuMusic(AudioClip clip)
    {
        menuMusic.clip = clip;
        menuMusic.Play();
        
    }
    
    
    
}
