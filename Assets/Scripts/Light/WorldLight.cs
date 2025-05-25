using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldLight : MonoBehaviour
{
    private Light2D _light;
    
    [SerializeField] private Gradient _gradient;
    
    private float currentPercentOfDay;
    private float targetPercentOfDay;

    private float smoothTime = 0.2f;
    private float currentSmoothVelocity = 50.0f; 
    private Color currentColor;
    private Color targetColor;

    private void Start()
    {
        _light = GetComponent<Light2D>();
        
        currentPercentOfDay = TimeManager.Instance.percentOfDay;
        targetPercentOfDay = currentPercentOfDay;
        currentColor = _gradient.Evaluate(currentPercentOfDay / 100f);
        _light.color = currentColor;
        
        TimeManager.Instance.WorldTimeChanged += OnWorldTimeChanged;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }

    private void OnWorldTimeChanged(float currentTime)
    {
        if (!TimeManager.Instance.isTimePaused)
        {
            targetPercentOfDay = TimeManager.Instance.percentOfDay;
        }
    }

    private void Update()
    {
        currentPercentOfDay = Mathf.SmoothDamp(currentPercentOfDay, targetPercentOfDay, ref currentSmoothVelocity, smoothTime);
        
        targetColor = _gradient.Evaluate(currentPercentOfDay / 100f);
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime / smoothTime);
        
        _light.color = currentColor;
    }
}