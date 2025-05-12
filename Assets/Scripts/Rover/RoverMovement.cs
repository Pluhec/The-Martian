using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TransmissionSystem), typeof(BoostSystem))]
public class RoverMovement : MonoBehaviour
{
    [Header("Speeds")]
    public float driveSpeed = 5f;
    public float reverseSpeed = 3f;
    public float turnSpeed = 100f;

    private Rigidbody2D rb;
    private TransmissionSystem transmission;
    private BoostSystem boost;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        transmission = GetComponent<TransmissionSystem>();
        boost = GetComponent<BoostSystem>();
    }

    private void FixedUpdate()
    {
        if (!transmission.isEngineOn)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return;
        }
        
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // nejde se otace pokud neni v pohybu
        if (Mathf.Abs(moveInput) > 0.1f)
            rb.angularVelocity = -turnInput * turnSpeed;
        else
            rb.angularVelocity = 0f;
        
        float speed = 0f;
        if (moveInput > 0f && transmission.currentGear == TransmissionSystem.Gear.D)
            speed = driveSpeed;
        else if (moveInput < 0f && transmission.currentGear == TransmissionSystem.Gear.R)
            speed = reverseSpeed;
        
        if (boost.IsBoosting)
            speed *= boost.consumptionMultiplier;
        
        Vector2 forward = transform.up * speed * moveInput;
        rb.linearVelocity = forward;
    }
}