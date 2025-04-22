using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceTrigger : MonoBehaviour
{
    [Header("Klíč & Cíl scény")]
    public string entranceKey;
    public string targetSceneName;

    [Header("Cross‑scene interakce")]
    public KeyCode activationKey = KeyCode.E;
    public bool    requireHold   = false;
    public float   holdDuration  = 0.5f;

    [Header("Same‑scene teleport")]
    public KeyCode directionKey           = KeyCode.None;
    public float   directionHoldDuration  = 0.1f;

    [Header("Efekt odletu z Habu")]
    public float   effectDuration = 1.3f;           // délka kouře + zvuku

    bool  playerInRange, hasTriggered;
    float holdTimer;

    /* ---------- TRIGGER ENTER / EXIT ---------- */

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;  hasTriggered = false;  holdTimer = 0f;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    /* ---------- UPDATE ---------- */

    void Update()
    {
        if (!playerInRange || hasTriggered) return;

        bool isSameScene = SceneManager.GetActiveScene().name == targetSceneName;

        if (isSameScene)           HandleSameSceneTeleport();
        else                       HandleCrossSceneTeleport();
    }

    void HandleSameSceneTeleport()
    {
        if (directionKey == KeyCode.None) return;

        if (Input.GetKey(directionKey))
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= directionHoldDuration) DoEnter();
        }
        else holdTimer = 0f;
    }

    void HandleCrossSceneTeleport()
    {
        if (requireHold)
        {
            if (Input.GetKey(activationKey))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdDuration) DoEnter();
            }
            else holdTimer = 0f;
        }
        else if (Input.GetKeyDown(activationKey))
        {
            DoEnter();
        }
    }

    /* ---------- HLAVNÍ AKCE ---------- */

    void DoEnter()
    {
        hasTriggered = true;

        EntranceData.Instance.lastEntranceKey = entranceKey;

        string current = SceneManager.GetActiveScene().name;
        bool marsToHab = current == "Mars" && targetSceneName == "Hab";
        bool habToMars = current == "Hab"  && targetSceneName == "Mars";

        if (habToMars)
        {
            Debug.Log("[Teleport] Odlet z Habu → Mars (log před loadem)");
            EntranceData.Instance.logAfterLoad = false;
            StartCoroutine(ExitWithEffectCo());   // spustíme kouř + zvuk → teprve pak LoadScene
        }
        else if (marsToHab)
        {
            EntranceData.Instance.logAfterLoad = true;   // log + efekt až v HabInterior
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

    /* ---------- KORUTINA ODLETU Z HABU ---------- */

    IEnumerator ExitWithEffectCo()
    {
        var effects = AirlockEffectManager.Instance;
        effects?.Play(entranceKey);

        FindObjectOfType<AudioManager>()?.PlayDecompressionSound();

        yield return new WaitForSeconds(effectDuration);

        effects?.Stop(entranceKey);

        SceneManager.LoadScene(targetSceneName);
    }
}