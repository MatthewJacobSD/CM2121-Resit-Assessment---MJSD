using System;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    private int currentScore;
    private int highScore;

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

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

    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();

        if (currentScore > highScore)
        {
            highScore = currentScore;
            OnHighScoreChanged?.Invoke(highScore);
            UpdateHighScoreUI();
        }
    }

    public void AddPenalty(int points)
    {
        currentScore -= points;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = currentScore.ToString();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText) highScoreText.text = highScore.ToString();
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore_Recycling", highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore_Recycling", 0);
        UpdateHighScoreUI();
    }
}