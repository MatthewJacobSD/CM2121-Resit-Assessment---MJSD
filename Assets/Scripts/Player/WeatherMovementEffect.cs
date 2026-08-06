using UnityEngine;

/// <summary>
/// Applies weather-based speed modifiers to the player and updates them live as
/// the weather state or storm intensity changes.
/// </summary>
public class WeatherMovementEffect : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Speed Modifiers")]
    [SerializeField] private float sunnySpeedMultiplier = 1.2f;
    [SerializeField] private float rainySpeedMultiplier = 0.75f;
    [Tooltip("Slowest speed during a full-strength storm.")]
    [SerializeField] private float stormySpeedMin = 0.45f;
    [Tooltip("Fastest speed at the start of a storm.")]
    [SerializeField] private float stormySpeedMax = 0.75f;

    #endregion

    #region Private Fields

    private float currentStormIntensity;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (weatherState != null)
            weatherState.OnWeatherChanged += OnWeatherChanged;
    }

    private void OnDisable()
    {
        if (weatherState != null)
            weatherState.OnWeatherChanged -= OnWeatherChanged;
    }

    private void Start()
    {
        if (weatherState != null && playerMovement != null)
            ApplyModifier(weatherState.GetCurrentState());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates the storm strength (0-1) and reapplies the modifier while stormy.
    /// </summary>
    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        if (weatherState != null && weatherState.GetCurrentState() == WeatherState.State.Stormy)
            ApplyModifier(WeatherState.State.Stormy);
    }

    #endregion

    #region Private Methods

    private void OnWeatherChanged(WeatherState.State newState)
    {
        ApplyModifier(newState);
    }

    private void ApplyModifier(WeatherState.State state)
    {
        if (playerMovement == null) return;

        float modifier = state switch
        {
            WeatherState.State.Sunny => sunnySpeedMultiplier,
            WeatherState.State.Rainy => rainySpeedMultiplier,
            WeatherState.State.HeavyRain => Mathf.Lerp(rainySpeedMultiplier, stormySpeedMax, 0.5f),
            WeatherState.State.Stormy => Mathf.Lerp(stormySpeedMax, stormySpeedMin, currentStormIntensity),
            _ => 1.0f
        };

        playerMovement.SetSpeedModifier(modifier);
    }

    #endregion
}
