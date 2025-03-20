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

    public float Stamina, MaxStamina;

    public float RunCost;
    public float RechargeRate;

    private Coroutine recharge;

    private Vector2 input;
    private Vector2 lastMoveDirection;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Initialize Stamina to full
        Stamina = MaxStamina;
    }

    void Update()
    {
        ProcessInputs();
        Animate();
        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {

        // Update StaminaPoints based on current Stamina (similar to health points in pomocnyscript)
        for (int i = 0; i < StaminaPoints.Length; i++)
        {
            StaminaPoints[i].enabled = DisplayStaminaPoints(Stamina, i);
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

    // Function to determine whether to show a stamina point
    bool DisplayStaminaPoints(float stamina, int pointNumber)
    {
        return ((pointNumber + 0.1) * (MaxStamina / StaminaPoints.Length) <= stamina);
    }
}