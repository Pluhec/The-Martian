using System.Collections;
using UnityEngine;
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
    public GameObject toastPrefab;
    public Transform notificationsParent;

    private Coroutine recharge;
    private float oxygenDepletionTimer = 0f;
    private Vector2 input;
    private Vector2 lastMoveDirection;
    private bool warningShown = false;
    private bool alertShown = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footstepManager = GetComponent<FootstepManager>();
        Stamina = MaxStamina;
        Oxygen = MaxOxygen;
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
            screenFade.color = Color.clear;
        }
    }

    private void UpdateStaminaBar()
    {
        for (int i = 0; i < StaminaPoints.Length; i++)
            StaminaPoints[i].enabled = ((i + 0.1f) * (MaxStamina / StaminaPoints.Length) <= Stamina);
    }

    private void UpdateOxygenBar()
    {
        if (Oxygen > 0)
        {
            Oxygen -= OxygenDepletionRate * Time.deltaTime;
            if (Oxygen < 0) Oxygen = 0;
            float ratio = Oxygen / MaxOxygen;
            OxygenBar.fillAmount = ratio;
            
            // posilani do popupu 
            if (!alertShown && ratio <= 0.05f)
            {
                var go = Instantiate(toastPrefab, notificationsParent);
                go.GetComponent<Toast>().Show("alert", $"Critical oxygen level! ({Mathf.RoundToInt(ratio * 100)}%)");
                alertShown = true;
            }
            else if (!warningShown && ratio <= 0.3f)
            {
                var go = Instantiate(toastPrefab, notificationsParent);
                go.GetComponent<Toast>().Show("warning", $"Your oxygen is low ({Mathf.RoundToInt(ratio * 100)}%)");
                warningShown = true;
            }
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

    private void ShowDeathMessage()
    {
        Debug.Log("You Died");
    }
}