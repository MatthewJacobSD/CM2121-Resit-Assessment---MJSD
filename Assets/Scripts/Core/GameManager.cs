using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private float gameDuration = 300f;
    [SerializeField] private int plantsRequired = 6;

    [Header("Scoring")]
    [SerializeField] private int plantScore = 10;
    [SerializeField] private int toyPenalty = 20;
    [SerializeField] private int bottlePenalty = 5;
    [SerializeField] private int chainBonus = 40;
    [SerializeField] private int chainThreshold = 2;

    private int lives;
    private float timeRemaining;
    private int itemsRecycled;
    private int plantsRecycled;
    private int toysRecycled;
    private int bottlesRecycled;
    private int consecutivePlants;
    private bool isPlaying;

    public int Lives => lives;
    public int MaxLives => maxLives;
    public float TimeRemaining => timeRemaining;
    public int ItemsRecycled => itemsRecycled;
    public int PlantsRecycled => plantsRecycled;
    public bool IsPlaying => isPlaying;

    public event Action OnGameStarted;
    public event Action OnGameOver;
    public event Action OnGameWon;
    public event Action<int, int> OnLivesChanged;
    public event Action<float> OnTimerTick;
    public event Action<int, string> OnItemRecycled;
    public event Action<string> OnAnnouncement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!isPlaying) return;

        timeRemaining -= Time.deltaTime;
        OnTimerTick?.Invoke(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            CheckWinCondition();
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        lives = maxLives;
        timeRemaining = gameDuration;
        itemsRecycled = 0;
        plantsRecycled = 0;
        toysRecycled = 0;
        bottlesRecycled = 0;
        consecutivePlants = 0;

        ScoreManager.Instance?.ResetScore();
        OnLivesChanged?.Invoke(lives, maxLives);
        OnGameStarted?.Invoke();
        OnAnnouncement?.Invoke("Collect plants and recycle them into the bin!");
    }

    public void ReportRecycled(GameObject item, int scoreValue)
    {
        if (!isPlaying) return;

        itemsRecycled++;

        PickupItem pickup = item.GetComponent<PickupItem>();
        ItemType type = pickup != null ? pickup.ItemType : ItemType.Plant;

        switch (type)
        {
            case ItemType.Plant:
                plantsRecycled++;
                consecutivePlants++;
                ScoreManager.Instance?.AddScore(plantScore);

                if (consecutivePlants >= chainThreshold)
                {
                    ScoreManager.Instance?.AddScore(chainBonus);
                    OnAnnouncement?.Invoke("Plant Chain! +" + chainBonus + " bonus!");
                }

                AudioManager.Instance?.PlaySuccessSFX();
                OnAnnouncement?.Invoke("Correct! Plant recycled +" + plantScore);
                break;

            case ItemType.Toy:
                toysRecycled++;
                consecutivePlants = 0;
                lives--;
                ScoreManager.Instance?.AddPenalty(toyPenalty);
                AudioManager.Instance?.PlayErrorSFX();
                OnAnnouncement?.Invoke("Wrong bin! Toys go in general waste. -" + toyPenalty);
                break;

            case ItemType.Bottle:
                bottlesRecycled++;
                consecutivePlants = 0;
                lives--;
                ScoreManager.Instance?.AddPenalty(bottlePenalty);
                AudioManager.Instance?.PlayErrorSFX();
                OnAnnouncement?.Invoke("Wrong bin! Bottles go in recycling. -" + bottlePenalty);
                break;
        }

        OnLivesChanged?.Invoke(lives, maxLives);
        OnItemRecycled?.Invoke(scoreValue, item.name);

        if (lives <= 0)
        {
            OnAnnouncement?.Invoke("No lives left!");
            EndGame(false);
        }
        else
        {
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (!isPlaying) return;

        if (plantsRecycled >= plantsRequired && ScoreManager.Instance.CurrentScore > 0)
        {
            EndGame(true);
        }
        else if (timeRemaining <= 0f)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool won)
    {
        isPlaying = false;
        ScoreManager.Instance?.SaveHighScore();

        if (won)
            OnGameWon?.Invoke();
        else
            OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
