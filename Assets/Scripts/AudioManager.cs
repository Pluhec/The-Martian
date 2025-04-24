using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource radialMenu;
    [SerializeField] AudioSource solarPanel;
    [SerializeField] AudioSource solarPanelDone;
    [SerializeField] AudioSource decompressionSource;
    [SerializeField] AudioSource SlidingAirlockDoorSource;
    [SerializeField] AudioSource footstepSource;
    
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
    
    [Header("------ Kroky ------")]
    public AudioClip walkingOnMetal;
    public AudioClip walkingOnSand;
    [Tooltip("Výška tónu při chůzi (výchozí 1)")]
    public float walkFootstepPitch = 1f;
    [Tooltip("Výška tónu při běhu (výchozí 1.1)")]
    public float runFootstepPitch = 1.1f;
    
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
    
    private AudioClip currentFootstep;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "WalkingOnMetal":
                currentFootstep = walkingOnMetal;
                break;
            case "WalkingOnSand":
                currentFootstep = walkingOnSand;
                break;
            default:
                currentFootstep = walkingOnMetal;
                break;
        }
    }

    public void PlayFootstep(bool isRunning)
    {
        if (currentFootstep == null || footstepSource == null)
            return;

        footstepSource.pitch = isRunning ? runFootstepPitch : walkFootstepPitch;
        footstepSource.PlayOneShot(currentFootstep);
    }

    
}
