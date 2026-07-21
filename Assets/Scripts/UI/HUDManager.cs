using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text remainingText;

    [Header("Score Popups")]
    [SerializeField] private GameObject plantScorePopup;
    [SerializeField] private GameObject toyScorePopup;
    [SerializeField] private GameObject plasticBottleScorePopup;

    [Header("Popup Timing")]
    [SerializeField] private float popupDuration = 1.5f;

    private float popupTimer;
    private GameObject activePopup;

    private void OnEnable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnItemRecycled += OnItemRecycled;
            GameManager.Instance.OnGameStarted += UpdateStats;
        }
        if (ScoreManager.Instance)
            ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnItemRecycled -= OnItemRecycled;
            GameManager.Instance.OnGameStarted -= UpdateStats;
        }
        if (ScoreManager.Instance)
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        HideAllPopups();
        UpdateStats();
    }

    private void Update()
    {
        if (popupTimer > 0)
        {
            popupTimer -= Time.deltaTime;
            if (popupTimer <= 0)
                HideAllPopups();
        }
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

    private void UpdateStats()
    {
        if (!GameManager.Instance) return;

        if (collectedText)
            collectedText.text = $"Collected: {GameManager.Instance.ItemsRecycled}";
        if (remainingText)
            remainingText.text = $"Remaining: {GameManager.Instance.RemainingItems}";
    }

    private void ShowPopup(string itemName)
    {
        HideAllPopups();

        if (itemName.Contains("Vase") || itemName.Contains("Bonsay"))
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
        if (plantScorePopup) plantScorePopup.SetActive(false);
        if (toyScorePopup) toyScorePopup.SetActive(false);
        if (plasticBottleScorePopup) plasticBottleScorePopup.SetActive(false);
        activePopup = null;
    }
}
