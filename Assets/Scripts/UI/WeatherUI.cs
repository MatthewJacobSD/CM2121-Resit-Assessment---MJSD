using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeatherUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI weatherStatusText;
    [SerializeField] private Image weatherIcon;

    public void UpdateWeatherUI(WeatherState.State currentState)
    {
        if (weatherStatusText != null)
            weatherStatusText.text = currentState.ToString().ToUpper();

        // TODO: Update weatherIcon.sprite based on state if needed
    }
}