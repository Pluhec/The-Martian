using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;

    [Header("Speeds")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;

    [Header("Footstep Timing")]
    [Tooltip("Interval between footsteps when walking (seconds)")]
    public float walkStepInterval = 0.5f;
    [Tooltip("Interval between footsteps when running (seconds)")]
    public float runStepInterval = 0.3f;

    private float footstepTimer;

    [Header("UI Elements")]
    public Image[] StaminaPoints;
    public Image OxygenBar;
    public Image screenFade;

    [Header("Stamina & Oxygen")]
    public float Stamina, MaxStamina;
    public float Oxygen, MaxOxygen;
    public float RunCost;
    public float RechargeRate;
    public float OxygenDepletionRate = 1f;
    public float OxygenTimeBeforeDeath = 10f;

    private Coroutine recharge;
    private float oxygenDepletionTimer = 0f;
    private Vector2 input;
    private Vector2 lastMoveDirection;
    public bool alive = true;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Stamina = MaxStamina;
        Oxygen = MaxOxygen;
        footstepTimer = Mathf.Max(walkStepInterval, runStepInterval);
    }

    void Update()
    {
        if (!alive)
            return;

        ProcessInputs();
        Animate();

        // Footstep logic
        bool isMoving = input.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Stamina > 0 && isMoving;
        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            float interval = isRunning ? runStepInterval : walkStepInterval;
            if (footstepTimer >= interval)
            {
                AudioManager.Instance.PlayFootstep(isRunning);
                footstepTimer = 0f;
            }
        }
        else
        {
            // reset timer so first step plays immediately when starting to move
            footstepTimer = isRunning ? runStepInterval : walkStepInterval;
        }

        UpdateStaminaBar();
        UpdateOxygenBar();

        if (Oxygen <= 0)
        {
            oxygenDepletionTimer += Time.deltaTime;
            screenFade.color = new Color(0, 0, 0, Mathf.Clamp01(oxygenDepletionTimer / OxygenTimeBeforeDeath));

            if (oxygenDepletionTimer >= OxygenTimeBeforeDeath)
            {
                alive = false;
                ShowDeathMessage();
            }
        }
        else
        {
            oxygenDepletionTimer = 0f;
            screenFade.color = new Color(0, 0, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        if (!alive)
            return;

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
        input = new Vector2(moveX, moveY).normalized;

        if (moveX != 0 || moveY != 0)
            lastMoveDirection = new Vector2(moveX, moveY);
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

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);
        while (Stamina < MaxStamina)
        {
            Stamina += RechargeRate / 10f;
            Stamina = Mathf.Min(Stamina, MaxStamina);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateStaminaBar()
    {
        for (int i = 0; i < StaminaPoints.Length; i++)
            StaminaPoints[i].enabled = DisplayStaminaPoints(Stamina, i);
    }

    private void UpdateOxygenBar()
    {
        if (Oxygen > 0)
        {
            Oxygen -= OxygenDepletionRate * Time.deltaTime;
            OxygenBar.fillAmount = Oxygen / MaxOxygen;
        }
        else
            Oxygen = 0;
    }

    private void ShowDeathMessage()
    {
        Debug.Log("You Died");
        // TODO: zobrazit UI zprávu
    }

    bool DisplayStaminaPoints(float stamina, int pointNumber)
    {
        return ((pointNumber + 0.1f) * (MaxStamina / StaminaPoints.Length) <= stamina);
    }
}
