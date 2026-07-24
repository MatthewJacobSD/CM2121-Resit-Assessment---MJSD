using UnityEngine;

public class WeatherState : MonoBehaviour
{
    public enum State
    {
        Sunny,
        Cloudy,
        Windy,
        Rainy,
        Stormy
    }

    [Header("Weather Cycle")]
    [SerializeField]
    private State[] weatherOrder =
    {
        State.Sunny, State.Cloudy, State.Windy, State.Rainy, State.Stormy
    };

    private int currentIndex = 0;
    public State CurrentWeather { get; private set; }

    public event System.Action<State> OnWeatherChanged;

    private void Start()
    {
        CurrentWeather = weatherOrder[0];
    }

    public void CycleWeather()
    {
        currentIndex = (currentIndex + 1) % weatherOrder.Length;
        CurrentWeather = weatherOrder[currentIndex];

        OnWeatherChanged?.Invoke(CurrentWeather);
    }

    public State GetCurrentState() => CurrentWeather;
}