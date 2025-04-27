using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Interval mezi kroky (v sekundách)")]
    [Tooltip("Interval mezi kroky při chůzi")]
    public float walkInterval = 0.5f;
    [Tooltip("Interval mezi kroky při běhu")]
    public float runInterval  = 0.3f;

    [Header("Pitch multiplikátor")]
    [Tooltip(">1 = rychlejší, <1 = pomalejší zvuk")]
    public float pitchMultiplier = 1f;

    private AudioManager audioManager;
    private Animator     animator;
    private float        timer;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        animator     = GetComponent<Animator>();
        timer        = 0f;
    }

    void Update()
    {
        // 1) zjistíme, jestli se pohybujeme a jestli běžíme
        float moveMag = animator.GetFloat("MoveMagnitude");
        bool  isRun   = animator.GetBool("IsRunning");

        // 2) když stojíme, resetujeme timer
        if (moveMag <= 0f)
        {
            timer = 0f;
            return;
        }

        // 3) přičítáme čas a čekáme na interval
        timer += Time.deltaTime;
        float interval = isRun ? runInterval : walkInterval;

        if (timer >= interval)
        {
            // 4) podle scény vybereme, kterou metodu volat
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mars")
                audioManager.PlaySandStep(1f, pitchMultiplier);
            else
                audioManager.PlayMetalStep(1f, pitchMultiplier);

            timer = 0f;
        }
    }
}