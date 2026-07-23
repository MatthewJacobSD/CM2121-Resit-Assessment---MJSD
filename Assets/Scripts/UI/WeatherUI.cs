using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeatherUI : MonoBehaviour
{
    // Reference to the TextMeshPro text element used to display
    // the current weather condition on the HUD.
    [SerializeField] private TextMeshProUGUI weatherStatusText;


    // Reference to the UI image used for displaying a weather icon.
    // This can later be updated with different sprites for each weather state.
    [SerializeField] private Image weatherIcon;


    // Updates the weather information shown on the UI.
    // Called whenever the weather state changes.
    public void UpdateWeatherUI(WeatherState.State currentState)
    {
        // Check that the text reference exists before updating it.
        if (weatherStatusText != null)
        {
            // Convert the weather state into uppercase text.
            // Example:
            // WeatherState.State.Sunny -> "SUNNY"
            weatherStatusText.text = currentState.ToString().ToUpper();
        }


        // Future implementation:
        // Change the weather icon depending on the current weather state.
        //
        // Example:
        // Sunny  -> Sun icon
        // Cloudy -> Cloud icon
        // Rainy  -> Rain icon
        // Stormy -> Storm icon
        //
        // weatherIcon.sprite = GetIconForState(currentState);
    }
}