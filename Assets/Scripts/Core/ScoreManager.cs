using System;
using UnityEngine;

/// <summary>
/// Tracks the current score and the persistent high score, persisting the
/// high score with PlayerPrefs and notifying listeners of changes.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    #region Constants

    private const string HighScoreKey = "HighScore_Recycling";

    #endregion

    #region Private Fields

    private int currentScore;
    private int highScore;

    #endregion

    #region Public Properties

    public static ScoreManager Instance { get; private set; }

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    #endregion

    #region Events

    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

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
        LoadHighScore();
    }

    #endregion

    #region Public Methods

    /// <summary>Adds points to the score and updates the high score if beaten.</summary>
    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            OnHighScoreChanged?.Invoke(highScore);
        }
    }

    /// <summary>Subtracts points (penalty) without allowing the score to go negative.</summary>
    public void AddPenalty(int points)
    {
        currentScore -= points;
        if (currentScore < 0) currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>Resets the current score to zero at the start of a game.</summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>Persists the current high score to PlayerPrefs.</summary>
    public void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    #endregion

    #region Private Methods

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    #endregion
}
