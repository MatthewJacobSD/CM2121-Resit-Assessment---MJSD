using System.Collections;
using UnityEngine;

/// <summary>
/// Drives weather visuals and audio: applies per-state effect settings, lerps
/// transitions, and crossfades ambient audio between weather states.
/// </summary>
public class WeatherEffects : MonoBehaviour
{
    #region Constants

    private const float DefaultCloudEmissionRate = 100f;
    private const float DefaultWindSpeed = 2f;

    #endregion

    #region Serialized Fields

    [Header("Visual Effects")]
    [SerializeField] private SunnyEffect sunnyEffect;
    [SerializeField] private CloudEffect cloudEffect;
    [SerializeField] private WindEffect windEffect;
    [SerializeField] private RainEffect rainEffect;
    [SerializeField] private LightingEffect lightingEffect;

    [Header("Weather Parameters")]
    [SerializeField] private WeatherEffectParameters sunnyParameters;
    [SerializeField] private WeatherEffectParameters rainyParameters;
    [SerializeField] private WeatherEffectParameters heavyRainParameters;
    [SerializeField] private WeatherEffectParameters stormyParameters;

    [Header("Ambient Audio")]
    [Tooltip("No clip assigned — sunny has no ambient weather audio.")]
    [SerializeField] private AudioClip sunnyAmbient;
    [SerializeField] private AudioClip rainyAmbient;
    [SerializeField] private AudioClip heavyRainAmbient;
    [SerializeField] private AudioClip stormyAmbient;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;

    [Header("Storm Intensity Ranges")]
    [SerializeField] private float rainMinIntensity = 200f;
    [SerializeField] private float rainMaxIntensity = 800f;
    [SerializeField] private float lightningActivationThreshold = 0.6f;

    #endregion

    #region Private Fields

    private WeatherState.State currentState;
    private Coroutine transitionRoutine;
    private float currentStormIntensity;
    private Color currentCloudColor = Color.grey;
    private float currentCloudEmission = DefaultCloudEmissionRate;
    private float currentWindSpeed = DefaultWindSpeed;

    #endregion

    #region Public Methods

    /// <summary>Applies the visual effects and ambient audio for a weather state.</summary>
    public void SetWeather(WeatherState.State state)
    {
        currentState = state;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionWeather(state));

        ApplyImmediateEffects(state);
        PlayAmbientAudio(state);
    }

    /// <summary>
    /// Adjusts storm strength (0-1), updating rain and lightning
    /// without triggering a full weather-state transition.
    /// </summary>
    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        if (rainEffect != null)
        {
            float rainIntensity = Mathf.Lerp(rainMinIntensity, rainMaxIntensity, currentStormIntensity);
            rainEffect.SetIntensity(rainIntensity);
        }

        if (lightingEffect != null)
            lightingEffect.SetActive(currentStormIntensity >= lightningActivationThreshold);
    }

    #endregion

    #region Weather Transitions

    private IEnumerator TransitionWeather(WeatherState.State state)
    {
        WeatherEffectParameters target = GetParametersForState(state);
        if (target == null) yield break;

        // Capture starting values for smooth lerp from current state.
        Color startColor = currentCloudColor;
        float startEmission = currentCloudEmission;
        float startWind = currentWindSpeed;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            if (cloudEffect != null)
            {
                Color targetColor = target.cloudColor;
                currentCloudColor = Color.Lerp(startColor, targetColor, t);
                cloudEffect.SetCloudColor(currentCloudColor);
                currentCloudEmission = Mathf.Lerp(startEmission, target.cloudEmissionRate, t);
                cloudEffect.SetEmissionRate(currentCloudEmission);
            }

            if (windEffect != null)
            {
                currentWindSpeed = Mathf.Lerp(startWind, target.windSpeed, t);
                windEffect.SetWindSpeed(currentWindSpeed);
            }

            yield return null;
        }

        // Snap to final values.
        currentCloudColor = target.cloudColor;
        currentCloudEmission = target.cloudEmissionRate;
        currentWindSpeed = target.windSpeed;
    }

    #endregion

    #region Effect State

    private void ApplyImmediateEffects(WeatherState.State state)
    {
        switch (state)
        {
            case WeatherState.State.Sunny:
                sunnyEffect?.SetActive(true);
                cloudEffect?.SetCloudy(false);
                rainEffect?.SetActive(false);
                rainEffect?.SetIntensity(0f);
                lightingEffect?.SetActive(false);
                break;

            case WeatherState.State.Rainy:
                sunnyEffect?.SetActive(false);
                cloudEffect?.SetCloudy(true);
                rainEffect?.SetActive(true);
                rainEffect?.SetIntensity(rainMinIntensity);
                lightingEffect?.SetActive(false);
                break;

            case WeatherState.State.HeavyRain:
                sunnyEffect?.SetActive(false);
                cloudEffect?.SetCloudy(true);
                rainEffect?.SetActive(true);
                rainEffect?.SetIntensity(Mathf.Lerp(rainMinIntensity, rainMaxIntensity, 0.5f));
                lightingEffect?.SetActive(false);
                break;

            case WeatherState.State.Stormy:
                sunnyEffect?.SetActive(false);
                cloudEffect?.SetCloudy(true);
                rainEffect?.SetActive(true);
                rainEffect?.SetIntensity(Mathf.Lerp(rainMinIntensity, rainMaxIntensity, currentStormIntensity));
                lightingEffect?.SetActive(currentStormIntensity >= lightningActivationThreshold);
                break;
        }
    }

    #endregion

    #region Ambient Audio

    private void PlayAmbientAudio(WeatherState.State state)
    {
        AudioClip clip = state switch
        {
            WeatherState.State.Sunny => sunnyAmbient,
            WeatherState.State.Rainy => rainyAmbient,
            WeatherState.State.HeavyRain => heavyRainAmbient,
            WeatherState.State.Stormy => stormyAmbient,
            _ => sunnyAmbient
        };

        // Sunny has no ambient weather audio — crossfade to silence.
        if (clip != null)
            AudioManager.Instance?.CrossfadeAmbient(clip);
        else
            AudioManager.Instance?.CrossfadeAmbient(null);
    }

    #endregion

    #region Utility

    private WeatherEffectParameters GetParametersForState(WeatherState.State state)
    {
        return state switch
        {
            WeatherState.State.Sunny => sunnyParameters,
            WeatherState.State.Rainy => rainyParameters,
            WeatherState.State.HeavyRain => heavyRainParameters ?? rainyParameters,
            WeatherState.State.Stormy => stormyParameters,
            _ => sunnyParameters
        };
    }

    #endregion
}
