using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicFader : MonoBehaviour
{
    [Tooltip("Délka fade-in/duration (v sekundách)")]
    public float fadeDuration = 1.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audioSource.volume = 0f;
        _audioSource.Play();
        StartCoroutine(FadeInCoroutine());
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        _audioSource.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVol = _audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
            yield return null;
        }
        _audioSource.volume = 0f;
        _audioSource.Stop();
    }
}