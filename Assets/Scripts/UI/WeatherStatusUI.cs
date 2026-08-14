using TMPro;
using UnityEngine;

/// <summary>
/// Drives the weather/status notification on the HUD: shows the current weather
/// state and the applied movement speed for a short time at game start and
/// whenever the weather changes, then hides the panel again.
/// </summary>
public class WeatherStatusUI : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("Panel GameObject shown/hidden for the notification.")]
    [SerializeField] private GameObject panel;

    [Tooltip("Text that displays the current weather state.")]
    [SerializeField] private TMP_Text statusText;

    [Tooltip("Text that displays the current walk speed in m/s.")]
    [SerializeField] private TMP_Text speedValueText;

    [SerializeField] private WeatherState weatherState;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Timing")]
    [Tooltip("How long the status panel stays visible after game start.")]
    [SerializeField] private float startupDuration = 4f;

    [Tooltip("How long the status panel stays visible after a weather change.")]
    [SerializeField] private float changeDuration = 3f;

    #endregion

    #region Private Fields

    private float visibleTimer;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStarted += OnGameStarted;

        if (weatherState != null)
            weatherState.OnWeatherChanged += OnWeatherChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStarted -= OnGameStarted;

        if (weatherState != null)
            weatherState.OnWeatherChanged -= OnWeatherChanged;
    }

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (visibleTimer > 0f)
        {
            visibleTimer -= Time.deltaTime;
            if (visibleTimer <= 0f && panel != null)
                panel.SetActive(false);
        }
    }

    #endregion

    #region Event Handlers

    private void OnGameStarted()
    {
        UpdateDisplay();
        ShowFor(startupDuration);
    }

    private void OnWeatherChanged(WeatherState.State newState)
    {
        UpdateDisplay();
        ShowFor(changeDuration);
    }

    #endregion

    #region Private Methods

    private void UpdateDisplay()
    {
        if (weatherState != null && statusText != null)
            statusText.text = FormatState(weatherState.GetCurrentState());

        if (playerMovement != null && speedValueText != null)
            speedValueText.text = $"{playerMovement.CurrentWalkSpeed:0.0} m/s";
    }

    private void ShowFor(float duration)
    {
        if (panel == null) return;

        panel.SetActive(true);
        visibleTimer = Mathf.Max(visibleTimer, duration);
    }

    private static string FormatState(WeatherState.State state)
    {
        return state switch
        {
            WeatherState.State.Sunny => "SUNNY",
            WeatherState.State.Rainy => "RAINY",
            WeatherState.State.HeavyRain => "HEAVY RAIN",
            WeatherState.State.Stormy => "STORMY",
            _ => "UNKNOWN"
        };
    }

    #endregion
}
