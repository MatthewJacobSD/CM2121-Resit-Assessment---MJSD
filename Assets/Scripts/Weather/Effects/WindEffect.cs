using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] private WindZone windZone;
    [SerializeField] private ParticleSystem windParticles;

    [Header("Per-State Settings")]
    [SerializeField] private float sunnyWindSpeed = 2f;
    [SerializeField] private float rainyWindSpeed = 8f;
    [SerializeField] private float stormyWindSpeedMin = 8f;
    [SerializeField] private float stormyWindSpeedMax = 20f;

    [SerializeField] private float maxWindSpeed = 25f;

    private float currentStormIntensity;

    private void Awake()
    {
        if (windZone == null)
            windZone = GetComponent<WindZone>();
    }

    public void SetWeatherState(WeatherState.State state)
    {
        float speed = state switch
        {
            WeatherState.State.Sunny => sunnyWindSpeed,
            WeatherState.State.Rainy => rainyWindSpeed,
            WeatherState.State.Stormy => Mathf.Lerp(stormyWindSpeedMin, stormyWindSpeedMax, currentStormIntensity),
            _ => 2f
        };

        SetWindSpeed(speed);
    }

    public void SetStormIntensity(float intensity)
    {
        currentStormIntensity = Mathf.Clamp01(intensity);

        float speed = Mathf.Lerp(stormyWindSpeedMin, stormyWindSpeedMax, currentStormIntensity);
        SetWindSpeed(speed);
    }

    public void SetActive(bool active)
    {
        float speed = active ? 12f : 0f;
        SetWindSpeed(speed);
    }

    public void SetWindSpeed(float speed)
    {
        float clamped = Mathf.Clamp(speed, 0f, maxWindSpeed);

        if (windZone != null)
            windZone.windMain = clamped;

        if (windParticles != null)
        {
            var em = windParticles.emission;
            em.rateOverTime = clamped * 4f;

            var main = windParticles.main;
            main.startSpeed = clamped * 0.5f;
        }
    }
}
