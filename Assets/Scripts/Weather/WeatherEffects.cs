using UnityEngine;
using System.Collections;

public class WeatherEffects : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private SunnyEffect sunnyEffect;
    [SerializeField] private CloudEffect cloudEffect;
    [SerializeField] private WindEffect windEffect;
    [SerializeField] private RainEffect rainEffect;
    [SerializeField] private LightingEffect lightingEffect;

    [Header("Weather Parameters")]
    [SerializeField] private WeatherEffectParameters sunnyParameters;
    [SerializeField] private WeatherEffectParameters rainyParameters;
    [SerializeField] private WeatherEffectParameters stormyParameters;

    [Header("Ambient Audio")]
    [SerializeField] private AudioClip sunnyAmbient;
    [SerializeField] private AudioClip rainyAmbient;
    [SerializeField] private AudioClip stormyAmbient;

    [Header("Storm Overlay")]
    [SerializeField] private GameObject stormOverlay;
    [SerializeField] private CanvasGroup stormOverlayAlpha;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;

    [Header("Storm Intensity Ranges")]
    [SerializeField] private float rainMinIntensity = 200f;
    [SerializeField] private float rainMaxIntensity = 800f;
    [SerializeField] private float stormOverlayMaxAlpha = 0.35f;
    [SerializeField] private float lightningActivationThreshold = 0.6f;

    private WeatherState.State currentState;
    private Coroutine transitionRoutine;
    private float currentStormIntensity;

    public void SetWeather(WeatherState.State state)
    {
        currentState = state;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(TransitionWeather(state));

        ApplyImmediateEffects(state);
        PlayAmbientAudio(state);
    }

    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        if (currentState != WeatherState.State.Stormy && currentStormIntensity > 0f)
        {
            ApplyImmediateEffects(WeatherState.State.Stormy);
            PlayAmbientAudio(WeatherState.State.Stormy);
        }

        if (rainEffect != null)
        {
            float rainIntensity = Mathf.Lerp(rainMinIntensity, rainMaxIntensity, currentStormIntensity);
            rainEffect.SetIntensity(rainIntensity);
        }

        if (lightingEffect != null)
            lightingEffect.SetActive(currentStormIntensity >= lightningActivationThreshold);

        SetStormOverlay(currentStormIntensity);
    }

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
                SetStormOverlay(0f);
                break;

            case WeatherState.State.Rainy:
                sunnyEffect?.SetActive(false);
                cloudEffect?.SetCloudy(true);
                rainEffect?.SetActive(true);
                rainEffect?.SetIntensity(rainMinIntensity);
                lightingEffect?.SetActive(false);
                SetStormOverlay(0f);
                break;

            case WeatherState.State.Stormy:
                sunnyEffect?.SetActive(false);
                cloudEffect?.SetCloudy(true);
                rainEffect?.SetActive(true);
                rainEffect?.SetIntensity(Mathf.Lerp(rainMinIntensity, rainMaxIntensity, currentStormIntensity));
                lightingEffect?.SetActive(currentStormIntensity >= lightningActivationThreshold);
                SetStormOverlay(currentStormIntensity);
                break;
        }
    }

    private IEnumerator TransitionWeather(WeatherState.State state)
    {
        WeatherEffectParameters target = GetParametersForState(state);
        if (target == null) yield break;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            if (cloudEffect != null)
            {
                Color targetColor = target.cloudColor;
                cloudEffect.SetCloudColor(Color.Lerp(Color.grey, targetColor, t));
                cloudEffect.SetEmissionRate(Mathf.Lerp(100f, target.cloudEmissionRate, t));
            }

            if (windEffect != null)
                windEffect.SetWindSpeed(Mathf.Lerp(2f, target.windSpeed, t));

            yield return null;
        }
    }

    private void SetStormOverlay(float intensity)
    {
        if (stormOverlay == null) return;

        if (intensity > 0f)
        {
            stormOverlay.SetActive(true);
            if (stormOverlayAlpha != null)
                stormOverlayAlpha.alpha = Mathf.Lerp(0f, stormOverlayMaxAlpha, intensity);
        }
        else
        {
            if (stormOverlayAlpha != null)
                stormOverlayAlpha.alpha = 0f;
            stormOverlay.SetActive(false);
        }
    }

    private void PlayAmbientAudio(WeatherState.State state)
    {
        AudioClip clip = state switch
        {
            WeatherState.State.Sunny => sunnyAmbient,
            WeatherState.State.Rainy => rainyAmbient,
            WeatherState.State.Stormy => stormyAmbient,
            _ => sunnyAmbient
        };
        if (clip != null)
            AudioManager.Instance?.CrossfadeAmbient(clip);
    }

    private WeatherEffectParameters GetParametersForState(WeatherState.State state)
    {
        return state switch
        {
            WeatherState.State.Sunny => sunnyParameters,
            WeatherState.State.Rainy => rainyParameters,
            WeatherState.State.Stormy => stormyParameters,
            _ => sunnyParameters
        };
    }
}
