using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
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

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    private InputAction continueAction;


    private enum PanelState { Welcome, Instructions, Playing, Ended }
    private PanelState currentState;

    private void Awake()
    {
        var uiMap = playerControls.FindActionMap("UI", true); // Make sure playerControls is assigned in Inspector
        continueAction = uiMap.FindAction("Continue", true);
    }

    private void Start()
    {
        ShowPanel(PanelState.Welcome);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += ShowEndScreen;
    }

    private void OnEnable()
    {
        continueAction.Enable();
        continueAction.performed += OnContinue;
    }

    private void OnDisable()
    {
        continueAction.performed -= OnContinue;
        continueAction.Disable();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowEndScreen;
    }

    private void ShowPanel(PanelState state)
    {
        currentState = state;

        welcomePanel?.SetActive(state == PanelState.Welcome);
        instructionPanel?.SetActive(state == PanelState.Instructions);
        hudPanel?.SetActive(state == PanelState.Playing);
        endPanel?.SetActive(state == PanelState.Ended);

        UpdateCursorState(state);
    }

    private void UpdateCursorState(PanelState state)
    {
        bool isPlaying = state == PanelState.Playing;
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;
    }

    private void OnContinue(InputAction.CallbackContext ctx)
    {
        if (currentState == PanelState.Welcome)
            ShowPanel(PanelState.Instructions);
        else if (currentState == PanelState.Instructions)
        {
            ShowPanel(PanelState.Playing);
            GameManager.Instance?.StartGame();
        }
    }

    private void ShowEndScreen()
    {
        ShowPanel(PanelState.Ended);

        int score = ScoreManager.Instance?.CurrentScore ?? 0;
        finalScoreText.text = $"Final Score: {score}";

        perfectCondition?.SetActive(score >= 30);
        defaultCondition?.SetActive(score > 0 && score < 30);
        failureCondition?.SetActive(score <= 0);

        congratulationText.text = score >= 30 ? "Perfect Cleanup!" :
                                 score > 0 ? "Good Effort!" : "Try Again!";
    }

    public void OnRestart()
    {
        GameManager.Instance?.RestartGame();
    }
}