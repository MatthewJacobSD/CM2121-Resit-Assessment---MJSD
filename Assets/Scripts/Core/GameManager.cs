using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game state manager: owns the timer, lives and win/lose conditions,
/// and exposes events that the rest of the game listens to.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Game Settings")]
    [Tooltip("Number of lives the player starts with. A wrong-bin recycle costs one life.")]
    [SerializeField] private int maxLives = 5;

    [Tooltip("Length of a single game in seconds.")]
    [SerializeField] private float gameDuration = 300f;

    [Tooltip("Plants that must be recycled to win the level.")]
    [SerializeField] private int plantsRequired = 6;

    [Header("Chain Bonus")]
    [Tooltip("Bonus score awarded when plants are recycled consecutively.")]
    [SerializeField] private int chainBonus = 40;

    [Tooltip("Consecutive plants required to trigger the chain bonus.")]
    [SerializeField] private int chainThreshold = 2;

    #endregion

    #region Private Fields

    private int lives;
    private float timeRemaining;
    private int itemsRecycled;
    private int plantsRecycled;
    private int toysRecycled;
    private int bottlesRecycled;
    private int consecutivePlants;
    private bool isPlaying;

    #endregion

    #region Public Properties

    public static GameManager Instance { get; private set; }

    public int Lives => lives;
    public int MaxLives => maxLives;
    public float TimeRemaining => timeRemaining;
    public int ItemsRecycled => itemsRecycled;
    public int PlantsRecycled => plantsRecycled;
    public int ToysRecycled => toysRecycled;
    public int BottlesRecycled => bottlesRecycled;
    public bool IsPlaying => isPlaying;

    #endregion

    #region Events

    public event Action OnGameStarted;
    public event Action OnGameOver;
    public event Action OnGameWon;
    public event Action<int, int> OnLivesChanged;
    public event Action<float> OnTimerTick;
    public event Action<int, string> OnItemRecycled;
    public event Action<string> OnAnnouncement;

    #endregion

    #region Unity Lifecycle

    // Singleton pattern: keep one persistent instance across scene reloads.
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

    #endregion

    #region Public Methods

    /// <summary>Resets all counters and starts a new game.</summary>
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

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        OnLivesChanged?.Invoke(lives, maxLives);
        OnGameStarted?.Invoke();
        OnAnnouncement?.Invoke("Collect plants and recycle them correctly!");
    }

    /// <summary>
    /// Registers a recycled item, applies scoring/lives effects and checks
    /// whether the game has been won or lost.
    /// </summary>
    /// <param name="item">The recycled item's GameObject.</param>
    /// <param name="scoreValue">Score granted for the recycled item.</param>
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

                if (consecutivePlants >= chainThreshold)
                {
                    OnAnnouncement?.Invoke($"Plant Chain! +{chainBonus} bonus!");
                }
                break;

            case ItemType.Toy:
                toysRecycled++;
                consecutivePlants = 0;
                lives--;
                break;

            case ItemType.Bottle:
                bottlesRecycled++;
                consecutivePlants = 0;
                lives--;
                break;
        }

        OnLivesChanged?.Invoke(lives, maxLives);
        OnItemRecycled?.Invoke(scoreValue, item.name);

        if (lives <= 0)
        {
            OnAnnouncement?.Invoke("Game Over - No lives left!");
            EndGame(false);
        }
        else
        {
            CheckWinCondition();
        }
    }

    /// <summary>Pauses gameplay updates while leaving the scene loaded.</summary>
    public void PauseGame()
    {
        if (!isPlaying) return;
        isPlaying = false;
    }

    /// <summary>Resumes gameplay updates after a pause.</summary>
    public void ResumeGame()
    {
        isPlaying = true;
    }

    /// <summary>Reloads the active scene to restart the game from scratch.</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Private Methods

    private void CheckWinCondition()
    {
        if (!isPlaying) return;

        if (plantsRecycled >= plantsRequired)
            EndGame(true);
        else if (timeRemaining <= 0f)
            EndGame(false);
    }

    private void EndGame(bool won)
    {
        isPlaying = false;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();

        if (won)
        {
            OnAnnouncement?.Invoke("Level Complete! Great Job!");
            OnGameWon?.Invoke();
        }
        else
        {
            OnGameOver?.Invoke();
        }
    }

    #endregion
}
