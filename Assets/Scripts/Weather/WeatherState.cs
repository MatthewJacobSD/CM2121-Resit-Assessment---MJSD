using UnityEngine;

public class WeatherState : MonoBehaviour
{
    public enum State
    {
        Sunny,
        Rainy,
        Stormy
    }

    [SerializeField] private State startingWeather = State.Sunny;

    public State CurrentWeather { get; private set; }

    public event System.Action<State> OnWeatherChanged;

    private void Start()
    {
        CurrentWeather = startingWeather;
    }

    public void SetWeather(State newState)
    {
        if (newState == CurrentWeather) return;
        CurrentWeather = newState;
        OnWeatherChanged?.Invoke(CurrentWeather);
    }

    public void SetSunny() => SetWeather(State.Sunny);
    public void SetRainy() => SetWeather(State.Rainy);
    public void SetStormy() => SetWeather(State.Stormy);

    public State GetCurrentState() => CurrentWeather;
}
