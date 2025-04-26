using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;

    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;

    public Image[] StaminaPoints; // Array for displaying stamina points
    public Image OxygenBar; // Oxygen bar
    public Image screenFade; // Image for screen fade effect (black)

    public float Stamina, MaxStamina;
    public float Oxygen, MaxOxygen; // New Oxygen variables
    public bool alive = true; // Alive status of the player

    public float RunCost;
    public float RechargeRate;
    public float OxygenDepletionRate = 1f; // Rate at which oxygen depletes
    public float OxygenTimeBeforeDeath = 10f; // Time before the player dies due to oxygen depletion

    private Coroutine recharge;
    private float oxygenDepletionTimer = 0f;

    private Vector2 input;
    private Vector2 lastMoveDirection;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Initialize Stamina and Oxygen to full
        Stamina = MaxStamina;
        Oxygen = MaxOxygen;
    }

    void Update()
    {
        if (!alive)
            return;

        ProcessInputs();
        Animate();
        UpdateStaminaBar();
        UpdateOxygenBar();

        // Handle oxygen depletion when oxygen is zero
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
            screenFade.color = new Color(0, 0, 0, 0); // Reset screen fade when oxygen is refilled
        }
    }

    private void UpdateStaminaBar()
    {
        // Update StaminaPoints based on current Stamina
        for (int i = 0; i < StaminaPoints.Length; i++)
        {
            StaminaPoints[i].enabled = DisplayStaminaPoints(Stamina, i);
        }
    }

    private void UpdateOxygenBar()
    {
        // Deplete oxygen over time if not refilled
        if (Oxygen > 0)
        {
            Oxygen -= OxygenDepletionRate * Time.deltaTime;
            OxygenBar.fillAmount = Oxygen / MaxOxygen;
        }
        else
        {
            Oxygen = 0;
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

    private void FixedUpdate()
    {
        if (!alive)
            return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && input.magnitude > 0;
        float speed = isRunning ? runSpeed : walkSpeed;

        body.linearVelocity = new Vector2(input.x * speed, input.y * speed);

        if (isRunning)
        {
            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0)
            {
                Stamina = 0;
                speed = walkSpeed;
                body.linearVelocity = new Vector2(input.x * speed, input.y * speed);
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
        {
            lastMoveDirection = new Vector2(moveX, moveY);
        }

        input.x = moveX;
        input.y = moveY;
        input.Normalize();
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
        // Display the death message (can replace this with UI text)
        Debug.Log("You Died");
        // Implement UI Text to display "You Died" here
    }

    // Function to determine whether to show a stamina point
    bool DisplayStaminaPoints(float stamina, int pointNumber)
    {
        return ((pointNumber + 0.1) * (MaxStamina / StaminaPoints.Length) <= stamina);
    }
}