using UnityEngine;

public class WeatherMovementEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Speed Modifiers")]
    [SerializeField] private float sunnySpeedMultiplier = 1.2f;
    [SerializeField] private float rainySpeedMultiplier = 0.75f;
    [SerializeField] private float stormySpeedMin = 0.45f;
    [SerializeField] private float stormySpeedMax = 0.75f;

    private float currentStormIntensity;

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

    private void OnWeatherChanged(WeatherState.State newState)
    {
        ApplyModifier(newState);
    }

    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        if (weatherState != null && weatherState.GetCurrentState() == WeatherState.State.Stormy)
            ApplyModifier(WeatherState.State.Stormy);
    }

    private void ApplyModifier(WeatherState.State state)
    {
        if (playerMovement == null) return;

        float modifier = state switch
        {
            WeatherState.State.Sunny => sunnySpeedMultiplier,
            WeatherState.State.Rainy => rainySpeedMultiplier,
            WeatherState.State.Stormy => Mathf.Lerp(stormySpeedMax, stormySpeedMin, currentStormIntensity),
            _ => 1.0f
        };

        playerMovement.SetSpeedModifier(modifier);
    }
}
