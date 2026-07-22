using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int RemainingItems => totalItems - itemsRecycled;
    public bool IsPlaying => isPlaying;


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
        FindRecyclableObjects();

        totalItems = recyclableObjects.Length;
        itemsRecycled = 0;

        Debug.Log($"Total recyclable objects: {totalItems}");
    }



    private void FindRecyclableObjects()
    {
        if (recyclableObjects == null || recyclableObjects.Length == 0)
        {
            recyclableObjects = GameObject.FindGameObjectsWithTag("Recyclable");
        }
    }



    public void StartGame()
    {
        isPlaying = true;

        itemsRecycled = 0;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();


        OnGameStarted?.Invoke();
    }



    public void ReportRecycled(GameObject item, int scoreValue)
    {
        if (!isPlaying)
            return;


        // Only recyclable objects increase progress
        if (item.CompareTag("Recyclable"))
        {
            itemsRecycled++;


            Debug.Log(
                $"Recycled: {item.name} | {itemsRecycled}/{totalItems}"
            );


            OnItemRecycled?.Invoke(scoreValue, item.name);



            if (itemsRecycled >= totalItems)
            {
                Debug.Log("All recyclable objects processed.");
                EndGame();
            }
        }
        else if (item.CompareTag("NonRecyclable"))
        {
            // Non recyclable objects do not affect completion
            Debug.Log(
                $"Incorrect item recycled: {item.name}"
            );


            OnItemRecycled?.Invoke(scoreValue, item.name);
        }
    }



    private void EndGame()
    {
        isPlaying = false;


        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();


        OnGameOver?.Invoke();
    }



    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}