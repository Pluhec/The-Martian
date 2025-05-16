using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void ExitGame()
    {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

    public void UpdateVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
    
    
    
}