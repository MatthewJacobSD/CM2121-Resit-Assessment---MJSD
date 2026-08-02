using TMPro;
using UnityEngine;

/// <summary>
/// Renders the in-game HUD: recycled-item counts, lives, timer, score, high
/// score, announcements and score popups. Auto-generates fallback texts when
/// serialized references are missing.
/// </summary>
public class HUDManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Stats")]
    [SerializeField] private TMP_Text collectedText;      // Plants
    [SerializeField] private TMP_Text toysText;
    [SerializeField] private TMP_Text bottlesText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Announcement")]
    [SerializeField] private TMP_Text announcementText;
    [SerializeField] private float announcementDuration = 3f;

    [Header("Score Popups")]
    [SerializeField] private GameObject plantScorePopup;
    [SerializeField] private GameObject toyScorePopup;
    [SerializeField] private GameObject plasticBottleScorePopup;

    [Header("Popup Settings")]
    [SerializeField] private float popupDuration = 1.5f;

    #endregion

    #region Private Fields

    private float popupTimer;
    private float announcementTimer;
    private GameObject activePopup;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnItemRecycled += OnItemRecycled;
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnLivesChanged += OnLivesChanged;
            GameManager.Instance.OnTimerTick += OnTimerTick;
            GameManager.Instance.OnAnnouncement += ShowAnnouncement;
        }

        if (ScoreManager.Instance)
        {
            ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
            ScoreManager.Instance.OnHighScoreChanged += OnHighScoreChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnItemRecycled -= OnItemRecycled;
            GameManager.Instance.OnGameStarted -= OnGameStarted;
            GameManager.Instance.OnLivesChanged -= OnLivesChanged;
            GameManager.Instance.OnTimerTick -= OnTimerTick;
            GameManager.Instance.OnAnnouncement -= ShowAnnouncement;
        }

        if (ScoreManager.Instance)
        {
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.OnHighScoreChanged -= OnHighScoreChanged;
        }
    }

    private void Start()
    {
        CreateFallbackTextsIfMissing();
        UpdateStats();
    }

    private void Update()
    {
        if (popupTimer > 0)
        {
            popupTimer -= Time.deltaTime;
            if (popupTimer <= 0) HideAllPopups();
        }

        if (announcementTimer > 0)
        {
            announcementTimer -= Time.deltaTime;
            if (announcementTimer <= 0 && announcementText != null)
                announcementText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Fallback Text Creation

    private void CreateFallbackTextsIfMissing()
    {
        if (announcementText == null)
            announcementText = CreateText("Announcement Text", new Vector2(0, 200), 28, Color.yellow);

        if (scoreText == null)
            scoreText = CreateText("Score Text", new Vector2(-300, 400), 24);

        if (highScoreText == null)
            highScoreText = CreateText("High Score Text", new Vector2(300, 400), 24);

        if (collectedText == null)
            collectedText = CreateText("Plants: 0", new Vector2(-400, 350), 22);

        if (toysText == null)
            toysText = CreateText("Toys: 0", new Vector2(-400, 300), 22);

        if (bottlesText == null)
            bottlesText = CreateText("Bottles: 0", new Vector2(-400, 250), 22);

        if (livesText == null)
            livesText = CreateText("Lives Text", new Vector2(400, 350), 22);

        if (timerText == null)
            timerText = CreateText("Timer Text", new Vector2(0, 450), 26, Color.white);
    }

    private TMP_Text CreateText(string defaultText, Vector2 anchoredPosition, int fontSize = 24, Color? color = null)
    {
        GameObject go = new GameObject(defaultText);
        go.transform.SetParent(transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(600, 60);

        TMP_Text text = go.AddComponent<TextMeshProUGUI>();
        text.text = defaultText;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color ?? Color.white;
        text.fontStyle = FontStyles.Bold;

        return text;
    }

    #endregion

    #region Event Handlers

    private void OnGameStarted()
    {
        UpdateStats();
        HideAllPopups();
    }

    private void OnItemRecycled(int score, string itemName)
    {
        UpdateStats();
        ShowPopup(itemName);
    }

    private void OnScoreChanged(int newScore)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {newScore}";
    }

    private void OnHighScoreChanged(int newHighScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"Best: {newHighScore}";
    }

    private void OnLivesChanged(int lives, int maxLives)
    {
        if (livesText != null)
            livesText.text = $"Lives: {lives}/{maxLives}";
    }

    private void OnTimerTick(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";

            // Colour-cue the player as time runs low.
            if (timeRemaining <= 30f)
                timerText.color = Color.red;
            else if (timeRemaining <= 60f)
                timerText.color = Color.yellow;
            else
                timerText.color = Color.white;
        }
    }

    #endregion

    #region Stats Updates

    private void UpdateStats()
    {
        if (GameManager.Instance == null) return;

        if (collectedText != null)
            collectedText.text = $"Plants: {GameManager.Instance.PlantsRecycled}";

        if (toysText != null)
            toysText.text = $"Toys: {GameManager.Instance.ToysRecycled}";

        if (bottlesText != null)
            bottlesText.text = $"Bottles: {GameManager.Instance.BottlesRecycled}";

        if (scoreText != null && ScoreManager.Instance != null)
            scoreText.text = $"Score: {ScoreManager.Instance.CurrentScore}";

        if (highScoreText != null && ScoreManager.Instance != null)
            highScoreText.text = $"Best: {ScoreManager.Instance.HighScore}";
    }

    #endregion

    #region Announcements & Popups

    private void ShowAnnouncement(string message)
    {
        if (announcementText == null) return;

        announcementText.text = message;
        announcementText.gameObject.SetActive(true);
        announcementTimer = announcementDuration;
    }

    private void ShowPopup(string itemName)
    {
        HideAllPopups();

        // Pick the popup that matches the recycled item type by its name.
        if (itemName.Contains("Vase") || itemName.Contains("Bonsai") || itemName.Contains("Plant"))
            activePopup = plantScorePopup;
        else if (itemName.Contains("DogPlushie") || itemName.Contains("Plushie"))
            activePopup = toyScorePopup;
        else if (itemName.Contains("PlasticBottle") || itemName.Contains("Bottle"))
            activePopup = plasticBottleScorePopup;

        if (activePopup != null)
        {
            activePopup.SetActive(true);
            popupTimer = popupDuration;
        }
    }

    private void HideAllPopups()
    {
        if (plantScorePopup) plantScorePopup.SetActive(false);
        if (toyScorePopup) toyScorePopup.SetActive(false);
        if (plasticBottleScorePopup) plasticBottleScorePopup.SetActive(false);
        activePopup = null;
    }

    #endregion
}
