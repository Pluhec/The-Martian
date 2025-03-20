using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace WorldTime
{
    [RequireComponent(typeof(Light2D))]
    public class WorldLight : MonoBehaviour
    {
        private Light2D _light;
    
        [SerializeField] private Gradient _gradient;

        private void Start()
        {
            TimeManager timeManager = TimeManager.Instance;
            if (timeManager != null)
            {
                timeManager.WorldTimeChanged += OnWorldTimeChanged;
            }
            else
            {
                Debug.LogError("TimeManager is not initialized. Cannot subscribe to WorldTimeChanged.");
            }

            _light = GetComponent<Light2D>();
        }

        private void OnDestroy()
        {
            TimeManager timeManager = TimeManager.Instance;
            if (timeManager != null)
            {
                timeManager.WorldTimeChanged -= OnWorldTimeChanged;
            }
        }
    
        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            _light.color = _gradient.Evaluate(PercentOfDay(newTime));
        }

        private float PercentOfDay(TimeSpan timeSpan)
        {
            return (float)timeSpan.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
        }
    }
}