using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeatherUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI weatherStatusText;
    [SerializeField] private Image weatherIcon;

    [Header("Sprites")]
    [SerializeField] private Sprite sunnyIcon;
    [SerializeField] private Sprite rainyIcon;
    [SerializeField] private Sprite stormyIcon;

    [Header("Emoji Fallback")]
    [SerializeField] private bool useEmojiFallback = true;
    [SerializeField] private float emojiFontSize = 52f;
    [SerializeField] private Color emojiColor = Color.white;

    [Header("References")]
    [SerializeField] private WeatherState weatherState;

    private TextMeshProUGUI emojiText;

    private void OnEnable()
    {
        if (weatherState != null)
            weatherState.OnWeatherChanged += UpdateWeatherUI;
    }

    private void OnDisable()
    {
        if (weatherState != null)
            weatherState.OnWeatherChanged -= UpdateWeatherUI;
    }

    private void Start()
    {
        if (weatherState != null)
            UpdateWeatherUI(weatherState.GetCurrentState());
    }

    public void UpdateWeatherUI(WeatherState.State currentState)
    {
        if (weatherStatusText != null)
            weatherStatusText.text = currentState.ToString().ToUpper();

        if (weatherIcon != null)
        {
            Sprite iconSprite = currentState switch
            {
                WeatherState.State.Sunny => sunnyIcon,
                WeatherState.State.Rainy => rainyIcon,
                WeatherState.State.Stormy => stormyIcon,
                _ => sunnyIcon
            };

            if (iconSprite != null)
            {
                weatherIcon.sprite = iconSprite;
                weatherIcon.gameObject.SetActive(true);
                HideEmoji();
            }
            else if (useEmojiFallback)
            {
                weatherIcon.sprite = null;
                weatherIcon.gameObject.SetActive(false);
                ShowEmoji(currentState);
            }
            else
            {
                weatherIcon.gameObject.SetActive(false);
            }
        }
    }

    private void ShowEmoji(WeatherState.State state)
    {
        if (emojiText == null)
            emojiText = CreateEmojiText();

        string emoji = state switch
        {
            WeatherState.State.Sunny => "\u2600\uFE0F",
            WeatherState.State.Rainy => "\uD83C\uDF27\uFE0F",
            WeatherState.State.Stormy => "\u26C8\uFE0F",
            _ => "\u2600\uFE0F"
        };

        emojiText.text = emoji;
        emojiText.gameObject.SetActive(true);
    }

    private void HideEmoji()
    {
        if (emojiText != null)
            emojiText.gameObject.SetActive(false);
    }

    private TextMeshProUGUI CreateEmojiText()
    {
        GameObject go = new("WeatherEmoji");
        go.transform.SetParent(transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, -70);
        rect.sizeDelta = new Vector2(120, 120);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = emojiFontSize;
        tmp.color = emojiColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = "";

        return tmp;
    }
}
