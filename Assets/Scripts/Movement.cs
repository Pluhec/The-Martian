using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;
    FootstepManager footstepManager;
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;
    public Image[] StaminaPoints;
    public Image OxygenBar;
    public Image screenFade;
    public float MaxStamina = 5f;
    public float MaxOxygen = 100f;
    public float Stamina;
    public float Oxygen;
    public bool alive = true;
    public float RunCost = 1f;
    public float RechargeRate = 5f;
    public float OxygenDepletionRate = 1f;
    public float OxygenTimeBeforeDeath = 10f;
    
    [Header("Death Screen UI")]
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private GameObject diedText;
    [SerializeField] private GameObject infoText;
    [SerializeField] private Button     backToMenuButton;
    
    private AudioManager audioManager;
    private MusicFader   musicFader;  
    private GameObject toastPrefab;
    private Transform notificationsParent;
    private Coroutine recharge;
    public float oxygenDepletionTimer = 0f;
    private Vector2 input;
    private Vector2 lastMoveDirection;
    private bool warningShown = false;
    private bool alertShown = false;
    private bool deathMusicPlayed = false;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footstepManager = GetComponent<FootstepManager>();
        Stamina = MaxStamina;
        Oxygen = MaxOxygen;
        
        var notifCanvas = GameObject.FindGameObjectWithTag("NotificationSystem");
        if (notifCanvas != null)
        {
            toastPrefab = notifCanvas.GetComponentInChildren<Toast>(true)?.gameObject;
            notificationsParent = notifCanvas.transform.Find("NotificationContainer") ?? notifCanvas.transform;
        }
        else
        {
            Debug.LogWarning("Notification canvas s tagem 'notificationSystem' nebyl nalezen.");
        }
        
        var audioObj = GameObject.FindWithTag("Audio");
        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
            musicFader   = audioObj.GetComponent<MusicFader>();
        }
    }

    void Update()
    {
        if (!alive) return;
        ProcessInputs();
        Animate();
        UpdateStaminaBar();
        UpdateOxygenBar();
    
        bool isRunningNow = Input.GetKey(KeyCode.LeftShift) && Stamina > 0;
        bool isMovingNow = input.magnitude > 0.1f;
    
        footstepManager?.SetMovementState(isMovingNow, isRunningNow);
    }

    private void UpdateStaminaBar()
    {
        for (int i = 0; i < StaminaPoints.Length; i++)
            StaminaPoints[i].enabled = ((i + 0.1f) * (MaxStamina / StaminaPoints.Length) <= Stamina);
    }

    private void UpdateOxygenBar()
    {
        if (SceneManager.GetActiveScene().name == "Hab")
            return;
        
        if (Oxygen > 0)
        {
            Oxygen -= OxygenDepletionRate * Time.deltaTime;
            if (Oxygen < 0) Oxygen = 0;
            float ratio = Oxygen / MaxOxygen;
            OxygenBar.fillAmount = ratio;
            
            if (!alertShown && ratio <= 0.05f)
            {
                if (toastPrefab != null && notificationsParent != null)
                {
                    var go = Instantiate(toastPrefab, notificationsParent);
                    go.GetComponent<Toast>().Show("alert", $"Critical oxygen level! ({Mathf.RoundToInt(ratio * 100)}%)");
                }
                alertShown = true;
            }
            else if (!warningShown && ratio <= 0.3f)
            {
                if (toastPrefab != null && notificationsParent != null)
                {
                    var go = Instantiate(toastPrefab, notificationsParent);
                    go.GetComponent<Toast>().Show("warning", $"Your oxygen is low ({Mathf.RoundToInt(ratio * 100)}%)");
                }
                warningShown = true;
            }
        }
        
        if (Oxygen <= 0)
        {
            if (oxygenDepletionTimer == 0f)
            {
                audioManager?.PlayDeathMusic();
                audioManager?.SetDeathMusicVolume(0f);
                deathMusicPlayed = true;
            }

            oxygenDepletionTimer += Time.deltaTime;
            screenFade.color = new Color(0, 0, 0, Mathf.Clamp01(oxygenDepletionTimer / OxygenTimeBeforeDeath));
            
            if (deathMusicPlayed && audioManager != null)
            {
                float normalizedTime = oxygenDepletionTimer / OxygenTimeBeforeDeath;
                int silaNarustu = 4;
                int plynuleZakonceni = 2;
                float volumePercent = Mathf.Clamp01(normalizedTime * normalizedTime * (silaNarustu - plynuleZakonceni * normalizedTime));
                audioManager.SetDeathMusicVolume(volumePercent);
            }

            if (oxygenDepletionTimer >= OxygenTimeBeforeDeath)
            {
                alive = false;
                ShowDeathMessage();
            }
        }
        else
        {
            oxygenDepletionTimer = 0f;
            screenFade.color = Color.clear;
            deathMusicPlayed = false;
        }
    }

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);
        while (Stamina < MaxStamina)
        {
            Stamina += RechargeRate / 10f;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player preferences byly vymazány při ukončení hry");
    }

    void FixedUpdate()
    {
        if (!alive) return;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && input.magnitude > 0;
        float speed = isRunning ? runSpeed : walkSpeed;
        body.linearVelocity = input * speed;
        if (isRunning)
        {
            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0)
            {
                Stamina = 0;
                body.linearVelocity = input * walkSpeed;
            }
            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
            lastMoveDirection = new Vector2(moveX, moveY);
        input = new Vector2(moveX, moveY).normalized;
    }

    void Animate()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Stamina > 0;
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
        anim.SetBool("IsRunning", isRunning);
    }
    
    public void ShowDeathMessage()
    {
        Debug.Log("You Died");
        if (!deathMusicPlayed)
            audioManager?.PlayDeathMusic();
        StartCoroutine(ShowDeathUI());
    }

    private IEnumerator ShowDeathUI()
    {
        yield return new WaitUntil(() => screenFade.color.a >= 1f);
        deathScreenPanel.SetActive(true);
        diedText.       SetActive(true);
        infoText.       SetActive(true);
        backToMenuButton.gameObject.SetActive(true);
    }

    public void ObaBackToMenu()
    {
        musicFader?.FadeOut();
        DestroyAllDontDestroyObjects();
        PlayerPrefs.DeleteAll();
        
        SpawnManager.Instance.LoadSceneFromMenu("StartMenu");
    }

    private void DestroyAllDontDestroyObjects()
    {
        GameObject tempObject = new GameObject("TempForSceneIdentification");
        DontDestroyOnLoad(tempObject);
        
        Scene dontDestroyScene = tempObject.scene;
        
        List<GameObject> rootObjects = new List<GameObject>();
        dontDestroyScene.GetRootGameObjects(rootObjects);
        
        foreach (var obj in rootObjects)
        {
            if (obj != tempObject)
            {
                Destroy(obj);
            }
        }
        
        Destroy(tempObject);
    }
}