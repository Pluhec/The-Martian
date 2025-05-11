using UnityEngine;

public class BoostSystem : MonoBehaviour
{
    [Header("Boost Settings")]
    public float boostDuration = 2f;
    public float boostCooldown = 5f;
    public float consumptionMultiplier = 2f;

    private float cooldownTimer = 0f;
    private bool boosting = false;

    public float CooldownRatio => Mathf.Clamp01(cooldownTimer / boostCooldown);

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }
    
    public void TryBoost()
    {
        if (cooldownTimer <= 0f && !boosting)
        {
            boosting = true;
            cooldownTimer = boostCooldown;
            Debug.Log("Boost: aktivovan");
            Invoke(nameof(EndBoost), boostDuration);
        }
    }

    private void EndBoost()
    {
        boosting = false;
        Debug.Log("Boost: skoncil");
    }
}