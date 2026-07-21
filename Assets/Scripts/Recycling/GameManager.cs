using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private GameObject[] recyclableObjects;

    private int totalItems;
    private int itemsRecycled;
    private bool isPlaying;

    public int TotalItems => totalItems;
    public int ItemsRecycled => itemsRecycled;
    public bool IsPlaying => isPlaying;
    public int RemainingItems => totalItems - itemsRecycled;

    public event Action OnGameStarted;
    public event Action OnGameOver;
    public event Action<int, string> OnItemRecycled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (recyclableObjects == null || recyclableObjects.Length == 0)
            recyclableObjects = GameObject.FindGameObjectsWithTag("Recyclable");

        totalItems = recyclableObjects.Length;
        itemsRecycled = 0;
    }

    public void StartGame()
    {
        isPlaying = true;
        itemsRecycled = 0;
        ScoreManager.Instance.ResetScore();
        OnGameStarted?.Invoke();
    }

    public void ReportRecycled(GameObject item, int scoreValue)
    {
        if (!isPlaying) return;

        itemsRecycled++;
        OnItemRecycled?.Invoke(scoreValue, item.name);

        if (itemsRecycled >= totalItems)
            EndGame();
    }

    private void EndGame()
    {
        isPlaying = false;
        ScoreManager.Instance.SaveHighScore();
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        ScoreManager.Instance.ResetScore();
        StartGame();
    }
}
