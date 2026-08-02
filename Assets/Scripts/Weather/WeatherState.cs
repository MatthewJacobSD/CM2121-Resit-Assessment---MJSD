using System;
using UnityEngine;

/// <summary>
/// Holds the current weather state and notifies listeners whenever it changes.
/// </summary>
public class WeatherState : MonoBehaviour
{
    #region Types

    public enum State
    {
        Sunny,
        Rainy,
        Stormy
    }

    #endregion

    #region Serialized Fields

    [Header("Weather")]
    [Tooltip("Weather state active at the start of the game.")]
    [SerializeField] private State startingWeather = State.Sunny;

    #endregion

    #region Public Properties

    /// <summary>The active weather state.</summary>
    public State CurrentWeather { get; private set; }

    #endregion

    #region Events

    /// <summary>Invoked with the new state whenever the weather changes.</summary>
    public event Action<State> OnWeatherChanged;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        CurrentWeather = startingWeather;
    }

    #endregion

    #region Public Methods

    /// <summary>Changes the weather and fires <see cref="OnWeatherChanged"/>.</summary>
    public void SetWeather(State newState)
    {
        if (newState == CurrentWeather) return;

        CurrentWeather = newState;
        OnWeatherChanged?.Invoke(CurrentWeather);
    }

    /// <summary>Sets the weather to sunny.</summary>
    public void SetSunny() => SetWeather(State.Sunny);

    /// <summary>Sets the weather to rainy.</summary>
    public void SetRainy() => SetWeather(State.Rainy);

    /// <summary>Sets the weather to stormy.</summary>
    public void SetStormy() => SetWeather(State.Stormy);

    /// <summary>Returns the current weather state.</summary>
    public State GetCurrentState() => CurrentWeather;

    #endregion
}
