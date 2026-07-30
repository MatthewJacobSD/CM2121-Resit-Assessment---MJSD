using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("Screen Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject endPanel;

    [Header("End Screen")]
    [SerializeField] private TMP_Text congratulationText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text endMessageText;
    [SerializeField] private GameObject perfectCondition;
    [SerializeField] private GameObject defaultCondition;
    [SerializeField] private GameObject failureCondition;

    [Header("Pause")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    private InputAction continueAction;
    private InputAction cancelAction;
    private InputAction pauseAction;
    private InputActionMap cachedPlayerMap;
    private InputActionMap cachedUIMap;

    private enum PanelState { Welcome, Instructions, Playing, Ended }
    private PanelState currentState;

    private void Awake()
    {
        cachedUIMap = playerControls.FindActionMap("UI", true);
        cachedPlayerMap = playerControls.FindActionMap("Player", true);
        continueAction = cachedUIMap.FindAction("Continue", true);
        cancelAction = cachedUIMap.FindAction("Cancel", true);
        pauseAction = cachedUIMap.FindAction("Pause", true);
    }

    private void Start()
    {
        ShowPanel(PanelState.Welcome);
    }

    private void OnEnable()
    {
        continueAction.Enable();
        continueAction.performed += OnContinue;

        if (cancelAction != null)
        {
            cancelAction.Enable();
            cancelAction.performed += OnCancel;
        }

        if (pauseAction != null)
        {
            pauseAction.performed += OnPause;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWon += OnGameWon;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
    }

    private void OnDisable()
    {
        continueAction.performed -= OnContinue;
        continueAction.Disable();

        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancel;
            cancelAction.Disable();
        }

        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
            pauseAction.Disable();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWon -= OnGameWon;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }
    }

    private void ShowPanel(PanelState state)
    {
        currentState = state;

        welcomePanel.SetActive(state == PanelState.Welcome);
        instructionPanel.SetActive(state == PanelState.Instructions);
        hudPanel.SetActive(state == PanelState.Playing);
        endPanel.SetActive(state == PanelState.Ended);

        UpdateCursorState(state);
        SwitchInputMap(state);
    }

    private void UpdateCursorState(PanelState state)
    {
        bool isPlaying = state == PanelState.Playing;
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;
    }

    private void SwitchInputMap(PanelState state)
    {
        switch (state)
        {
            case PanelState.Welcome:
            case PanelState.Instructions:
                cachedPlayerMap.Disable();
                cachedUIMap.Enable();
                if (pauseAction != null) pauseAction.Disable();
                break;

            case PanelState.Playing:
                cachedUIMap.Disable();
                cachedPlayerMap.Enable();
                if (pauseAction != null) pauseAction.Enable();
                break;

            case PanelState.Ended:
                cachedPlayerMap.Disable();
                cachedUIMap.Enable();
                if (pauseAction != null) pauseAction.Disable();
                break;
        }
    }

    private void OnContinue(InputAction.CallbackContext ctx)
    {
        if (currentState == PanelState.Welcome)
            ShowPanel(PanelState.Instructions);
        else if (currentState == PanelState.Instructions)
        {
            ShowPanel(PanelState.Playing);
            GameManager.Instance.StartGame();
        }
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (currentState == PanelState.Instructions)
            ShowPanel(PanelState.Welcome);
    }

    private void OnGameWon()
    {
        AudioManager.Instance.PlayAchievementSFX();
        ShowEndScreen("Perfect Cleanup!", "You recycled all the plants and saved the environment!");
    }

    private void OnGameOver()
    {
        ShowEndScreen("Game Over", "Time ran out or you lost all your lives. Try again!");
    }

    private void ShowEndScreen(string title, string message)
    {
        ShowPanel(PanelState.Ended);

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        int lives = GameManager.Instance != null ? GameManager.Instance.Lives : 0;

        finalScoreText.text = $"Final Score: {score}";
        congratulationText.text = title;

        if (endMessageText != null)
            endMessageText.text = message;

        perfectCondition.SetActive(score >= 30 && lives > 0);
        defaultCondition.SetActive(score > 0 && score < 30);
        failureCondition.SetActive(score <= 0);
    }

    public void OnRestart()
    {
        GameManager.Instance.RestartGame();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (currentState == PanelState.Playing)
            ShowPauseMenu();
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cachedPlayerMap.Disable();
        cachedUIMap.Enable();
        if (pauseAction != null) pauseAction.Disable();
    }

    public void ReturnFromPause()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cachedUIMap.Disable();
        cachedPlayerMap.Enable();
        if (pauseAction != null) pauseAction.Enable();
    }
}
