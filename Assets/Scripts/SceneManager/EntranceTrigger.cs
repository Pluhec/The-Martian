using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceTrigger : MonoBehaviour
{
    [Header("Klíč & Cíl scény")]
    public string entranceKey;
    public string targetSceneName;

    [Header("Cross‑scene interakce")]
    public KeyCode activationKey = KeyCode.E;
    public bool requireHold = false;
    public float holdDuration = 0.5f;

    [Header("Same‑scene teleport")]
    public KeyCode directionKey = KeyCode.None;
    public float directionHoldDuration = 0.1f;

    bool playerInRange;
    bool hasTriggered;
    float holdTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        hasTriggered = false;
        holdTimer = 0f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange || hasTriggered) return;

        bool isSameScene = SceneManager.GetActiveScene().name == targetSceneName;

        if (isSameScene)
        {
            // teleporovani v ramci jedne sceny - musi drzet klavesu (je to podle pohybu)
            if (directionKey == KeyCode.None) return;

            if (Input.GetKey(directionKey))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= directionHoldDuration)
                    DoEnter();
            }
            else
            {
                holdTimer = 0f;
            }
        }
        else
        {
            // prechod do jine sceny - musi stisknout klavesu (mozna pridam radial menu)
            if (requireHold)
            {
                if (Input.GetKey(activationKey))
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= holdDuration)
                        DoEnter();
                }
                else
                {
                    holdTimer = 0f;
                }
            }
            else
            {
                if (Input.GetKeyDown(activationKey))
                    DoEnter();
            }
        }
    }

    private void DoEnter()
    {
        hasTriggered = true;

        // ulozeni klice pro SpawnManager (aby vedel kam ho potom spawnout)
        EntranceData.Instance.lastEntranceKey = entranceKey;

        // pokud jsme ve stejne scene, tak neloadi scenu znovu, ale pouze teleportuje
        if (SceneManager.GetActiveScene().name == targetSceneName)
        {
            SpawnManager.Instance.TeleportPlayer(entranceKey);
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}