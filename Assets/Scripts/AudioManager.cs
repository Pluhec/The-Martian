using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource radialMenu;
    
    [Header("------ RadialMenu ------")]
    public AudioClip openCloseMenu;
    public AudioClip howerAction;
    public AudioClip doAction;
    
    public void PlayRadialMenu(AudioClip clip)
    {
        radialMenu.PlayOneShot(clip);
    }
}
