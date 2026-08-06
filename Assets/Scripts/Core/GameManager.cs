using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Outcome of a finished game, used to drive the end screen.</summary>
public enum GameResult
{
    /// <summary>Every category's required count was recycled correctly.</summary>
    Perfect,
    /// <summary>The game ended (time out) with a positive score.</summary>
    Default,
    /// <summary>Lost all lives, ran out of time with no score, or went negative.</summary>
    Failure
}

/// <summary>
/// Central game state manager: owns the timer, lives, category objectives and
/// win/lose conditions, and exposes events the rest of the game listens to.
///
/// Scoring is wired through <see cref="ScoreManager"/> with signed values so a
/// wrong-bin recycle reduces the score (and can push it negative, which is an
/// immediate failure). Category progress is only granted for correct recycles.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Game Settings")]
    [Tooltip("Number of lives the player starts with. A wrong-bin recycle costs one life.")]
    [SerializeField] private int maxLives = 5;

    [Tooltip("Length of a single game in seconds.")]
    [SerializeField] private float gameDuration = 300f;

    [Tooltip("Plants that must be recycled correctly to win the level.")]
    [SerializeField] private int plantsRequired = 12;

    [Tooltip("Toys that must be recycled correctly to win the level.")]
    [SerializeField] private int toysRequired = 8;

    [Tooltip("Bottles that must be recycled correctly to win the level.")]
    [SerializeField] private int bottlesRequired = 4;

    [Header("Chain Bonus")]
    [Tooltip("Bonus score awarded when the plant chain threshold is reached.")]
    [SerializeField] private int chainBonus = 40;

    [Tooltip("Consecutive correct plants required to trigger the chain bonus.")]
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
    private GameResult result;

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

    /// <summary>Required correct recycles per category (HUD progress exposure).</summary>
    public int PlantsRequired => plantsRequired;
    public int ToysRequired => toysRequired;
    public int BottlesRequired => bottlesRequired;

    public bool IsPlaying => isPlaying;
    public GameResult Result => result;

    #endregion

    #region Events

    public event Action OnGameStarted;
    public event Action OnGameOver;      // Failure
    public event Action OnGameWon;       // Perfect or Default (kept for compatibility)
    public event Action<GameResult> OnGameEnded;
    public event Action<int, int> OnLivesChanged;
    public event Action<float> OnTimerTick;
    public event Action<int, string> OnItemRecycled;
    public event Action<string> OnAnnouncement;

    #endregion

    #region Unity Lifecycle

    // Singleton pattern: keep one persistent instance across scene reloads.
    // DontDestroyOnLoad only accepts root GameObjects. The scene keeps
    // managers under a "Managers" container, so persistence is skipped there
    // (each scene load recreates the managers; the high score survives via
    // ScoreManager's PlayerPrefs). Root-created instances (tests) still persist.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (transform.parent == null)
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
            CheckEndCondition();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>Resets all counters and starts a new game.</summary>
    public void StartGame()
    {
        isPlaying = true;
        result = GameResult.Failure;
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
        OnAnnouncement?.Invoke("Recycle all the plants, toys and bottles into the right bins!");
    }

    /// <summary>
    /// Registers a recycled item. The signed <paramref name="scoreValue"/> comes
    /// from the bin's scoring matrix: positive = correct, negative = wrong.
    /// Correct recycles advance the matching category; wrong ones cost a life and
    /// reduce the score. The game may end Perfect / Default / Failure.
    /// </summary>
    /// <param name="item">The recycled item's GameObject.</param>
    /// <param name="scoreValue">Signed score granted for the recycled item.</param>
    public void ReportRecycled(GameObject item, int scoreValue)
    {
        if (!isPlaying) return;

        itemsRecycled++;

        PickupItem pickup = item != null ? item.GetComponent<PickupItem>() : null;
        ItemType type = pickup != null ? pickup.ItemType : ItemType.Plant;

        bool correct = scoreValue > 0;

        switch (type)
        {
            case ItemType.Plant:
                if (correct)
                {
                    plantsRecycled++;
                    HandlePlantChain();
                }
                break;

            case ItemType.Toy:
                if (correct) toysRecycled++;
                break;

            case ItemType.Bottle:
                if (correct) bottlesRecycled++;
                break;
        }

        // Signed score wiring: correct recycles add, wrong ones subtract.
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        if (!correct)
        {
            consecutivePlants = 0;
            lives--;
            OnLivesChanged?.Invoke(lives, maxLives);
        }

        OnItemRecycled?.Invoke(scoreValue, item != null ? item.name : "Item");

        if (lives <= 0)
        {
            OnAnnouncement?.Invoke("Game Over - No lives left!");
            EndGame(GameResult.Failure);
            return;
        }

        if (ScoreManager.Instance != null && ScoreManager.Instance.CurrentScore < 0)
        {
            OnAnnouncement?.Invoke("Game Over - Score dropped below zero!");
            EndGame(GameResult.Failure);
            return;
        }

        CheckEndCondition();
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

    /// <summary>Grants and announces the plant chain bonus on a consecutive streak.</summary>
    private void HandlePlantChain()
    {
        consecutivePlants++;

        // Fire bonus every chainThreshold consecutive correct plant recycles.
        if (consecutivePlants >= chainThreshold && consecutivePlants % chainThreshold == 0)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(chainBonus);

            OnAnnouncement?.Invoke($"Plant Chain! +{chainBonus} bonus!");
        }
    }

    private bool AllCategoriesComplete()
    {
        return plantsRecycled >= plantsRequired
            && toysRecycled >= toysRequired
            && bottlesRecycled >= bottlesRequired;
    }

    private GameResult ScoreBasedResult()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        return score > 0 ? GameResult.Default : GameResult.Failure;
    }

    private void CheckEndCondition()
    {
        if (!isPlaying) return;

        if (AllCategoriesComplete())
        {
            EndGame(GameResult.Perfect);
        }
        else if (timeRemaining <= 0f)
        {
            EndGame(ScoreBasedResult());
        }
    }

    private void EndGame(GameResult gameResult)
    {
        if (!isPlaying) return;
        isPlaying = false;
        result = gameResult;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();

        switch (gameResult)
        {
            case GameResult.Perfect:
                OnAnnouncement?.Invoke("Perfect Cleanup! All items recycled!");
                OnGameWon?.Invoke();
                break;

            case GameResult.Default:
                OnAnnouncement?.Invoke("Level Complete! Good job!");
                OnGameWon?.Invoke();
                break;

            case GameResult.Failure:
                OnGameOver?.Invoke();
                break;
        }

        OnGameEnded?.Invoke(gameResult);
    }

    #endregion
}
