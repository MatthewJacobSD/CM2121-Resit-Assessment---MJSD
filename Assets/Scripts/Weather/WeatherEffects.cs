using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
    [Header("Weather Parameters")]
    [SerializeField] private WeatherEffectParameters sunnyParameters;
    [SerializeField] private WeatherEffectParameters cloudyParameters;
    [SerializeField] private WeatherEffectParameters windyParameters;
    [SerializeField] private WeatherEffectParameters rainyParameters;
    [SerializeField] private WeatherEffectParameters stormyParameters;

    [Header("Effects")]
    [SerializeField] private SunnyEffect sunnyEffect;
    [SerializeField] private CloudEffect cloudEffect;
    [SerializeField] private WindEffect windEffect;
    [SerializeField] private RainEffect rainEffect;
    [SerializeField] private LightingEffect lightingEffect;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerFootstepAudio footstepAudio;

    private WeatherEffectParameters currentParameters;
    private WeatherEffectParameters targetParameters;

    private void Awake()
    {
        currentParameters = new WeatherEffectParameters(sunnyParameters);
        targetParameters = new WeatherEffectParameters();
    }

    public void SetWeather(WeatherState.State state)
    {
        // Set target parameters based on weather state
        targetParameters = state switch
        {
            WeatherState.State.Sunny => sunnyParameters,
            WeatherState.State.Cloudy => cloudyParameters,
            WeatherState.State.Windy => windyParameters,
            WeatherState.State.Rainy => rainyParameters,
            WeatherState.State.Stormy => stormyParameters,
            _ => sunnyParameters
        };

        // Update visual effects immediately
        ApplyImmediateEffects(state);

        // Start smooth transition
        StartCoroutine(TransitionToWeather());
    }

    private void ApplyImmediateEffects(WeatherState.State state)
    {
        // Footsteps
        bool isWet = state == WeatherState.State.Rainy || state == WeatherState.State.Stormy;
        if (footstepAudio != null)
            footstepAudio.SetSurface(isWet ? PlayerFootstepAudio.SurfaceType.WetGrass : PlayerFootstepAudio.SurfaceType.DryGrass);

        // Special effects
        lightingEffect?.SetActive(state == WeatherState.State.Stormy);
        sunnyEffect?.SetActive(state == WeatherState.State.Sunny);
    }

    private System.Collections.IEnumerator TransitionToWeather()
    {
        float transitionTime = 2.5f;
        float elapsed = 0f;

        WeatherEffectParameters startParams = new WeatherEffectParameters(currentParameters);

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            currentParameters = LerpParameters(startParams, targetParameters, t);
            ApplyParameters(currentParameters);

            yield return null;
        }

        currentParameters = new WeatherEffectParameters(targetParameters);
        ApplyParameters(currentParameters);
    }

    private WeatherEffectParameters LerpParameters(WeatherEffectParameters a, WeatherEffectParameters b, float t)
    {
        return new WeatherEffectParameters
        {
            cloudColor = Color.Lerp(a.cloudColor, b.cloudColor, t),
            cloudEmissionRate = Mathf.Lerp(a.cloudEmissionRate, b.cloudEmissionRate, t),
            rainEmissionRate = Mathf.Lerp(a.rainEmissionRate, b.rainEmissionRate, t),
            windSpeed = Mathf.Lerp(a.windSpeed, b.windSpeed, t),
            lightingActive = b.lightingActive,
            sunRaysActive = b.sunRaysActive
        };
    }

    private void ApplyParameters(WeatherEffectParameters p)
    {
        if (cloudEffect != null)
        {
            cloudEffect.SetCloudColor(p.cloudColor);
            // You can add cloudEmissionRate support in CloudEffect if needed
        }

        rainEffect?.SetIntensity(p.rainEmissionRate);
        windEffect?.SetWindSpeed(p.windSpeed);
    }
}