using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceTrigger : MonoBehaviour
{
    [Header("Klíč & Cíl scény")] public string entranceKey;
    public string targetSceneName;

    [Header("Cross-scene interakce")] public KeyCode activationKey = KeyCode.E;
    public bool requireHold;
    public float holdDuration = 0.5f;

    [Header("Same-scene teleport")] public KeyCode directionKey = KeyCode.None;
    public float directionHoldDuration = 0.1f;

    [Header("Jak dlouho budou trvat particly")]
    public float effectDuration = 1.3f;

    [Header("Prodleva pred nebo po PS")] public float extraEffectDelay = 0.7f;

    private bool playerInRange, hasTriggered;
    private float holdTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        hasTriggered = false;
        holdTimer = 0f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange || hasTriggered) return;

        var isSameScene = SceneManager.GetActiveScene().name == targetSceneName;
        if (isSameScene) HandleSameSceneTeleport();
        else HandleCrossSceneTeleport();
    }

    private void HandleSameSceneTeleport()
    {
        if (directionKey == KeyCode.None) return;
        if (Input.GetKey(directionKey))
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= directionHoldDuration) DoEnter();
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private void HandleCrossSceneTeleport()
    {
        if (requireHold)
        {
            if (Input.GetKey(activationKey))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdDuration) DoEnter();
            }
            else
            {
                holdTimer = 0f;
            }
        }
        else if (Input.GetKeyDown(activationKey))
        {
            DoEnter();
        }
    }

    private void DoEnter()
    {
        hasTriggered = true;
        EntranceData.Instance.lastEntranceKey = entranceKey;

        var current = SceneManager.GetActiveScene().name;
        var habToMars = current == "Hab" && targetSceneName == "Mars";
        var marsToHab = current == "Mars" && targetSceneName == "Hab";

        if (habToMars)
        {
            EntranceData.Instance.logAfterLoad = false;
            StartCoroutine(ExitWithEffectCo());
        }
        else if (marsToHab)
        {
            EntranceData.Instance.logAfterLoad = true;
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            EntranceData.Instance.logAfterLoad = false;
            if (current == targetSceneName)
                SpawnManager.Instance.TeleportPlayer(entranceKey);
            else
                SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator ExitWithEffectCo()
    {
        var player    = GameObject.FindWithTag("Player");
        var movement  = player?.GetComponent<Movement>();
        var animator  = player?.GetComponent<Animator>();
        
        if (movement != null) 
            movement.enabled = false;
        if (animator != null)
        {
            animator.Play("Idle", 0, 0f);
            animator.SetFloat("MoveMagnitude", 0);
            animator.SetFloat("MoveY", 0);
            animator.SetFloat("MoveX", 0);
            Debug.Log("animator by mel fungovat");   
        } 
        
        // spust kour a zvuk
        var effects = AirlockEffectManager.Instance;
        effects?.ControlSmoke(entranceKey, effectDuration);
        FindObjectOfType<AudioManager>()?.PlayDecompressionSound();

        // pocka az dojedou efekty
        yield return new WaitForSeconds(effectDuration + extraEffectDelay);
        
        if (movement != null) 
            movement.enabled = true;

        SceneManager.LoadScene(targetSceneName);
    }
}