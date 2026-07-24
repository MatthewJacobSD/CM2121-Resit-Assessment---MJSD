using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private WeatherTimer weatherTimer;
    [SerializeField] private WeatherEffects weatherEffects;

    private void OnEnable()
    {
        weatherTimer.OnTimerExpired += OnTimerExpired;
        weatherState.OnWeatherChanged += OnWeatherChanged;
    }

    private void OnDisable()
    {
        weatherTimer.OnTimerExpired -= OnTimerExpired;
        weatherState.OnWeatherChanged -= OnWeatherChanged;
    }

    private void OnTimerExpired()
    {
        weatherState.CycleWeather();
    }

    private void OnWeatherChanged(WeatherState.State newState)
    {
        weatherEffects.SetWeather(newState);
    }
}