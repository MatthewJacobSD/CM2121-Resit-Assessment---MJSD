using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public WeatherTimer weatherTimer;
    public WeatherState weatherState;
    public WeatherEffects weatherEffects;

    void OnEnable()
    {
        weatherTimer.OnTimerExpired += HandleTimerExpired;
        weatherState.OnWeatherChanged += HandleWeatherChanged;
    }

    private void OnDisable()
    {
        weatherTimer.OnTimerExpired -= HandleTimerExpired;
        weatherState.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void HandleTimerExpired()
    {
        weatherState.CycleWeatherState();
    }

    private void HandleWeatherChanged(WeatherState.State state)
    {
        weatherEffects.SetWeatherEffect(state);
    }
}