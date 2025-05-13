using UnityEngine;
using System.Collections;

public class RoverAudioController : MonoBehaviour
{
    public AudioClip startSound;
    public AudioClip stopSound;
    public AudioClip idleSound;
    public AudioClip goingSound;
    public AudioClip boostSound;
    public AudioClip shiftSound;

    // Proste mi nefungujou audioSourcy
    [Range(0f, 1f)] public float startVolume = 1f;
    [Range(0f, 1f)] public float stopVolume = 1f;
    [Range(0f, 1f)] public float idleVolume = 1f;
    [Range(0f, 1f)] public float goingVolume = 1f;
    [Range(0f, 1f)] public float boostVolume = 1f;
    [Range(0f, 1f)] public float shiftVolume = 1f;

    private AudioSource engineSource;
    private AudioSource effectsSource;
    private TransmissionSystem transmission;
    private BoostSystem boost;
    private bool wasEngineOn = false;

    void Start()
    {
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.loop = true;

        effectsSource = gameObject.AddComponent<AudioSource>();
        
        transmission = GetComponent<TransmissionSystem>();
        boost = GetComponent<BoostSystem>();

        wasEngineOn = transmission.isEngineOn;
    }

    void Update()
    {
        if (transmission.isEngineOn != wasEngineOn)
        {
            wasEngineOn = transmission.isEngineOn;
            if (wasEngineOn)
            {
                StartCoroutine(EngineStartSequence());
            }
            else
            {
                StartCoroutine(EngineStopSequence());
            }
        }
        
        if (!transmission.isEngineOn) return;
        
        if (boost.IsBoosting)
        {
            PlayBoost();
        }
        else if (transmission.currentGear == TransmissionSystem.Gear.D ||
                transmission.currentGear == TransmissionSystem.Gear.R)
        {
            PlayGoing();
        }
        else 
        {
            PlayIdle();
        }
    }

    private IEnumerator EngineStartSequence()
    {
        effectsSource.volume = startVolume;
        effectsSource.PlayOneShot(startSound);

        yield return new WaitForSeconds(startSound.length);

        PlayIdle();
    }

    private IEnumerator EngineStopSequence()
    {
        PlayIdle();

        yield return new WaitForSeconds(0.2f);
        
        effectsSource.volume = stopVolume;
        effectsSource.PlayOneShot(stopSound);

        engineSource.Stop();
    }

    private void PlayIdle()
    {
        if (engineSource.clip != idleSound || !engineSource.isPlaying)
        {
            engineSource.clip = idleSound;
            engineSource.volume = idleVolume;
            engineSource.Play();
        }
    }

    private void PlayGoing()
    {
        if (engineSource.clip != goingSound || !engineSource.isPlaying)
        {
            engineSource.clip = goingSound;
            engineSource.volume = goingVolume;
            engineSource.Play();
        }
    }

    private void PlayBoost()
    {
        if (engineSource.clip != boostSound || !engineSource.isPlaying)
        {
            engineSource.clip = boostSound;
            engineSource.volume = boostVolume;
            engineSource.Play();
        }
    }

    public void PlayShiftSound()
    {
        effectsSource.volume = shiftVolume;
        effectsSource.PlayOneShot(shiftSound);
    }
}