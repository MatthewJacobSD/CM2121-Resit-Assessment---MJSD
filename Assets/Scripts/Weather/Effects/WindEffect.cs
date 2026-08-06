using UnityEngine;

/// <summary>
/// Drives the wind zone and wind particles, with per-weather-state speeds and
/// a storm intensity that scales wind from calm to maximum strength.
/// </summary>
public class WindEffect : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("Wind zone that pushes objects. Falls back to the component's own WindZone.")]
    [SerializeField] private WindZone windZone;
    [SerializeField] private ParticleSystem windParticles;

    [Header("Per-State Settings")]
    [SerializeField] private float sunnyWindSpeed = 2f;
    [SerializeField] private float rainyWindSpeed = 8f;
    [SerializeField] private float stormyWindSpeedMin = 8f;
    [SerializeField] private float stormyWindSpeedMax = 20f;

    [SerializeField] private float maxWindSpeed = 25f;

    #endregion

    #region Private Fields

    private float currentStormIntensity;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (windZone == null)
            windZone = GetComponent<WindZone>();
    }

    #endregion

    #region Public Methods

    /// <summary>Applies the wind speed configured for the given weather state.</summary>
    public void SetWeatherState(WeatherState.State state)
    {
        float speed = state switch
        {
            WeatherState.State.Sunny => sunnyWindSpeed,
            WeatherState.State.Rainy => rainyWindSpeed,
            WeatherState.State.HeavyRain => Mathf.Lerp(rainyWindSpeed, stormyWindSpeedMin, 0.5f),
            WeatherState.State.Stormy => Mathf.Lerp(stormyWindSpeedMin, stormyWindSpeedMax, currentStormIntensity),
            _ => 2f
        };

        SetWindSpeed(speed);
    }

    /// <summary>Scales storm wind strength in [0, 1].</summary>
    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        float speed = Mathf.Lerp(stormyWindSpeedMin, stormyWindSpeedMax, currentStormIntensity);
        SetWindSpeed(speed);
    }

    /// <summary>Turns the wind on (12) or off (0).</summary>
    public void SetActive(bool active)
    {
        float speed = active ? 12f : 0f;
        SetWindSpeed(speed);
    }

    /// <summary>Applies a wind speed to the zone and particle emission/velocity.</summary>
    public void SetWindSpeed(float speed)
    {
        float clamped = Mathf.Clamp(speed, 0f, maxWindSpeed);

        if (windZone != null)
            windZone.windMain = clamped;

        if (windParticles != null)
        {
            ParticleSystem.EmissionModule em = windParticles.emission;
            em.rateOverTime = clamped * 4f;

            ParticleSystem.MainModule main = windParticles.main;
            main.startSpeed = clamped * 0.5f;
        }
    }

    #endregion
}
