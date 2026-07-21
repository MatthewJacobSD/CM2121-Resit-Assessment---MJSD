using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasRoot;

    [Header("Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject endPanel;

    [Header("End Screen")]
    [SerializeField] private TMP_Text congratulationText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private GameObject perfectCondition;
    [SerializeField] private GameObject defaultCondition;
    [SerializeField] private GameObject failureCondition;

    [Header("End Screen Buttons")]
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject endGameButton;

    private enum PanelState { Welcome, Instructions, Playing, Ended }
    private PanelState currentState;

    private void Start()
    {
        if (canvasRoot) canvasRoot.SetActive(true);
        ShowPanel(PanelState.Welcome);
    }

    private void Update()
    {
        if (currentState == PanelState.Welcome || currentState == PanelState.Instructions)
        {
            if (Input.anyKeyDown)
                AdvancePanel();
        }
    }

    private void AdvancePanel()
    {
        switch (currentState)
        {
            case PanelState.Welcome:
                ShowPanel(PanelState.Instructions);
                break;
            case PanelState.Instructions:
                ShowPanel(PanelState.Playing);
                GameManager.Instance.StartGame();
                break;
        }
    }

    private void ShowPanel(PanelState state)
    {
        currentState = state;
        if (welcomePanel) welcomePanel.SetActive(state == PanelState.Welcome);
        if (instructionPanel) instructionPanel.SetActive(state == PanelState.Instructions);
        if (hudPanel) hudPanel.SetActive(state == PanelState.Playing);
        if (endPanel) endPanel.SetActive(state == PanelState.Ended);
    }

    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnGameOver += ShowEndScreen;
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.OnGameOver -= ShowEndScreen;
    }

    private void ShowEndScreen()
    {
        ShowPanel(PanelState.Ended);

        int score = ScoreManager.Instance.CurrentScore;
        int total = GameManager.Instance.TotalItems;

        if (finalScoreText) finalScoreText.text = $"Final Score: {score}";

        if (perfectCondition) perfectCondition.SetActive(false);
        if (defaultCondition) defaultCondition.SetActive(false);
        if (failureCondition) failureCondition.SetActive(false);

        if (score >= 30)
        {
            if (perfectCondition) perfectCondition.SetActive(true);
            if (congratulationText) congratulationText.text = "Perfect Cleanup!";
        }
        else if (score > 0)
        {
            if (defaultCondition) defaultCondition.SetActive(true);
            if (congratulationText) congratulationText.text = "Good Effort!";
        }
        else
        {
            if (failureCondition) failureCondition.SetActive(true);
            if (congratulationText) congratulationText.text = "Try Again!";
        }
    }

    public void OnRestart()
    {
        GameManager.Instance.RestartGame();
        ShowPanel(PanelState.Playing);
    }

    public void OnEndGame()
    {
        Application.Quit();
    }
}
