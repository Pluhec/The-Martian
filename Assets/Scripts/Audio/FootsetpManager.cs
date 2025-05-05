using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sandSource;
    [SerializeField] private AudioSource metalSource;

    [Header("Footstep Clips")]
    [SerializeField] private AudioClip[] sandFootsteps;
    [SerializeField] private AudioClip[] metalFootsteps;

    [Header("Step Timing")]
    [SerializeField] private float walkStepRate = 0.6f;
    [SerializeField] private float runStepRate = 0.35f;

    [Header("Settings")]
    public bool isOutside = true;

    private float stepTimer = 0f;
    private bool isMoving = false;
    private bool isRunning = false;
    private bool wasRunning = false;

    void Update()
    {
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            // Detect state change (walk <-> run)
            if (isRunning != wasRunning)
            {
                InterruptAndPlayNewStep();
                stepTimer = isRunning ? runStepRate : walkStepRate;
                wasRunning = isRunning;
            }

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = isRunning ? runStepRate : walkStepRate;
            }
        }
        else
        {
            stepTimer = 0f;
            StopPlaying(); // Stop footstep when not moving
        }
    }

    public void SetMovementState(bool moving, bool running)
    {
        isMoving = moving;
        isRunning = running;
    }

    private void InterruptAndPlayNewStep()
    {
        StopPlaying();
        PlayFootstep(); // immediately play a new step on mode switch
    }

    private void StopPlaying()
    {
        if (sandSource.isPlaying) sandSource.Stop();
        if (metalSource.isPlaying) metalSource.Stop();
    }

    private void PlayFootstep()
    {
        if (isOutside)
        {
            if (sandFootsteps.Length == 0) return;
            AudioClip clip = sandFootsteps[Random.Range(0, sandFootsteps.Length)];
            sandSource.PlayOneShot(clip);
        }
        else
        {
            if (metalFootsteps.Length == 0) return;
            AudioClip clip = metalFootsteps[Random.Range(0, metalFootsteps.Length)];
            metalSource.PlayOneShot(clip);
        }
    }
}