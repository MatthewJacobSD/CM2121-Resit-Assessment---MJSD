using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text timerText;

    [Header("Announcement")]
    [SerializeField] private TMP_Text announcementText;
    [SerializeField] private float announcementDuration = 3f;

    [Header("Score Popups")]
    [SerializeField] private GameObject plantScorePopup;
    [SerializeField] private GameObject toyScorePopup;
    [SerializeField] private GameObject plasticBottleScorePopup;

    [Header("Popup Settings")]
    [SerializeField] private float popupDuration = 1.5f;

    private float popupTimer;
    private float announcementTimer;
    private GameObject activePopup;

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
            ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
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
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
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
        UpdateStats();
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

            if (timeRemaining <= 30f)
                timerText.color = Color.red;
            else if (timeRemaining <= 60f)
                timerText.color = Color.yellow;
            else
                timerText.color = Color.white;
        }
    }

    private void UpdateStats()
    {
        if (!GameManager.Instance) return;

        if (collectedText != null)
            collectedText.text = $"Plants: {GameManager.Instance.PlantsRecycled}";
    }

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

        if (itemName.Contains("Vase") || itemName.Contains("Bonsai"))
            activePopup = plantScorePopup;
        else if (itemName.Contains("DogPlushie"))
            activePopup = toyScorePopup;
        else if (itemName.Contains("PlasticBottle"))
            activePopup = plasticBottleScorePopup;

        if (activePopup)
        {
            activePopup.SetActive(true);
            popupTimer = popupDuration;
        }
    }

    private void HideAllPopups()
    {
        plantScorePopup?.SetActive(false);
        toyScorePopup?.SetActive(false);
        plasticBottleScorePopup?.SetActive(false);
        activePopup = null;
    }
}
