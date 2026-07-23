using System;
using System.Collections;
using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
    private WeatherEffectParameters currentWeatherEffectParameters;
    private WeatherEffectParameters targetWeatherEffectParameters;

    [SerializeField] WeatherEffectParameters sunnyWeatherParameters;
    [SerializeField] WeatherEffectParameters cloudyWeatherParameters;
    [SerializeField] WeatherEffectParameters windyWeatherParameters;
    [SerializeField] WeatherEffectParameters rainyWeatherParameters;
    [SerializeField] WeatherEffectParameters stormyWeatherParameters;

    [SerializeField] SunnyEffect sunnyEffect;
    [SerializeField] CloudEffect cloudEffect;
    [SerializeField] WindEffect windEffect;
    [SerializeField] RainEffect rainEffect;
    [SerializeField] LightingEffect lightingEffect;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] FootstepAudio footstepAudio;

    private void Awake()
    {
        currentWeatherEffectParameters = sunnyWeatherParameters;
        targetWeatherEffectParameters = gameObject.AddComponent<WeatherEffectParameters>();
    }

    private void Start()
    {
        SetWeatherEffect(WeatherState.State.Sunny);
    }

    public void SetWeatherEffect(WeatherState.State weatherState)
    {
        switch (weatherState)
        {
            case WeatherState.State.Sunny:
                targetWeatherEffectParameters = sunnyWeatherParameters;
                cloudEffect.SetSunny();
                break;

            case WeatherState.State.Cloudy:
                targetWeatherEffectParameters = cloudyWeatherParameters;
                cloudEffect.SetCloudy();
                break;

            case WeatherState.State.Rainy:
                targetWeatherEffectParameters = rainyWeatherParameters;
                cloudEffect.SetCloudy();
                break;

            case WeatherState.State.Stormy:
                targetWeatherEffectParameters = stormyWeatherParameters;
                cloudEffect.SetStormy();
                break;
        }

        footstepAudio.SetSurface(
            weatherState == WeatherState.State.Rainy ||
            weatherState == WeatherState.State.Stormy
                ? FootstepAudio.SurfaceType.WetGrass
                : FootstepAudio.SurfaceType.DryGrass
        );

        StartCoroutine(TransitionToNextEffect());
    }

    private IEnumerator TransitionToNextEffect()
    {
        float transitionTime = 3f;
        float elapsedTime = 0;

        WeatherEffectParameters startWeatherEffectParameters = currentWeatherEffectParameters;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionTime);

            currentWeatherEffectParameters = LerpWeatherEffectParameters(startWeatherEffectParameters, targetWeatherEffectParameters, t);
            UpdateWeatherEffects(currentWeatherEffectParameters);
            yield return null;
        }
        currentWeatherEffectParameters = targetWeatherEffectParameters;
        UpdateWeatherEffects(currentWeatherEffectParameters);
    }

    private WeatherEffectParameters LerpWeatherEffectParameters(WeatherEffectParameters from, WeatherEffectParameters to, float t)
    {
        WeatherEffectParameters result = new()
        {
            cloudColor = Color.Lerp(from.cloudColor, to.cloudColor, t),
            cloudEmissionRate = Mathf.Lerp(from.cloudEmissionRate, to.cloudEmissionRate, t),
            rainEmissionRate = Mathf.Lerp(from.rainEmissionRate, to.rainEmissionRate, t),
            windSpeed = Mathf.Lerp(from.windSpeed, to.windSpeed, t),
            lightingActive = to.lightingActive,
            sunRaysActive = to.sunRaysActive
        };
        return result;
    }

    private void UpdateWeatherEffects(WeatherEffectParameters weatherEffectParameters)
    {
        cloudEffect.SetCloudDarkness(weatherEffectParameters.cloudColor);
        cloudEffect.SetCloudEmissionRate(weatherEffectParameters.cloudEmissionRate);
        rainEffect.SetRainIntensity(weatherEffectParameters.rainEmissionRate, playerMovement);
        windEffect.SetWindSpeed(weatherEffectParameters.windSpeed);

        if (weatherEffectParameters.lightingActive) lightingEffect.ActiveLightingEffect();
        else lightingEffect.DeactivateLightingEffect();

        if (weatherEffectParameters.sunRaysActive) sunnyEffect.ActiveSunnyEffect();
        else sunnyEffect.DeactivateSunnyEffect();
    }

    public WeatherEffectParameters GetCurrentWeatherEffectParameters()
    {
        return currentWeatherEffectParameters;
    }
    public SunnyEffect GetSunnyEffect() { return sunnyEffect; }

    public CloudEffect GetCloudEffect() { return cloudEffect; }

    public WindEffect GetWindEffect() { return windEffect; }

    public RainEffect GetRainEffect() { return rainEffect; }

    public LightingEffect GetLightingEffect() { return lightingEffect; }
}