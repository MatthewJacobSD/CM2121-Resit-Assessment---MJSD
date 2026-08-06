using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Drives the main menu flow (welcome, instructions, gameplay, end screen),
/// switches input maps between UI and gameplay, and coordinates the pause menu.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Screen Panels")]
    [Tooltip("First screen shown; pressing continue advances to the instructions.")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject endPanel;

    [Header("End Screen")]
    [SerializeField] private TMP_Text congratulationText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text endMessageText;
    [Tooltip("Shown when every category's required count was recycled correctly.")]
    [SerializeField] private GameObject perfectCondition;
    [Tooltip("Shown when the game ended with a positive score but not perfect.")]
    [SerializeField] private GameObject defaultCondition;
    [Tooltip("Shown when the player failed (no lives, no score, or negative score).")]
    [SerializeField] private GameObject failureCondition;

    [Header("Pause")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Input")]
    [Tooltip("Input Action Asset containing the Player and UI action maps.")]
    [SerializeField] private InputActionAsset playerControls;

    #endregion

    #region Private Fields

    private InputAction continueAction;
    private InputAction cancelAction;
    private InputAction playerPauseAction;
    private InputActionMap cachedPlayerMap;
    private InputActionMap cachedUIMap;
    private PauseMenuManager pauseMenu;

    private enum PanelState { Welcome, Instructions, Playing, Ended }
    private PanelState currentState;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        cachedUIMap = playerControls.FindActionMap("UI", true);
        cachedPlayerMap = playerControls.FindActionMap("Player", true);
        continueAction = cachedUIMap.FindAction("Continue", true);
        cancelAction = cachedUIMap.FindAction("Cancel", true);

        // Pause lives on the Player map so Escape is reachable during gameplay.
        playerPauseAction = cachedPlayerMap.FindAction("Pause");

        pauseMenu = FindFirstObjectByType<PauseMenuManager>();
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

        if (playerPauseAction != null)
        {
            playerPauseAction.performed += OnPause;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded += OnGameEnded;
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

        if (playerPauseAction != null)
        {
            playerPauseAction.performed -= OnPause;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded -= OnGameEnded;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>Restarts the game by reloading the active scene.</summary>
    public void OnRestart()
    {
        GameManager.Instance.RestartGame();
    }

    /// <summary>Shows the pause menu and switches to UI input, hiding gameplay controls.</summary>
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cachedPlayerMap.Disable();
        cachedUIMap.Enable();
    }

    /// <summary>Hides the pause menu and returns to full gameplay input.</summary>
    public void ReturnFromPause()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cachedUIMap.Disable();
        cachedPlayerMap.Enable();
    }

    #endregion

    #region Panel Navigation

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
                break;

            case PanelState.Playing:
                cachedUIMap.Disable();
                cachedPlayerMap.Enable();
                break;

            case PanelState.Ended:
                cachedPlayerMap.Disable();
                cachedUIMap.Enable();
                break;
        }
    }

    #endregion

    #region End Screen

    private void OnGameEnded(GameResult result)
    {
        ShowEndScreen(result);
    }

    private void ShowEndScreen(GameResult result)
    {
        ShowPanel(PanelState.Ended);

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        int lives = GameManager.Instance != null ? GameManager.Instance.Lives : 0;

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {score}";

        switch (result)
        {
            case GameResult.Perfect:
                congratulationText.text = "Perfect Cleanup!";
                SetEndMessage("You recycled all the plants, toys and bottles correctly. The park is saved!");
                SetConditions(true, false, false);
                break;

            case GameResult.Default:
                congratulationText.text = "Level Complete!";
                SetEndMessage("Good job! You kept the park clean before time ran out.");
                SetConditions(false, true, false);
                break;

            case GameResult.Failure:
                congratulationText.text = "Game Over";
                SetEndMessage(lives <= 0
                    ? "You ran out of lives. Try again!"
                    : "Time ran out or your score dropped below zero. Try again!");
                SetConditions(false, false, true);
                break;
        }
    }

    private void SetEndMessage(string message)
    {
        if (endMessageText != null)
            endMessageText.text = message;
    }

    private void SetConditions(bool perfect, bool moderate, bool failure)
    {
        if (perfectCondition != null) perfectCondition.SetActive(perfect);
        if (defaultCondition != null) defaultCondition.SetActive(moderate);
        if (failureCondition != null) failureCondition.SetActive(failure);
    }

    #endregion

    #region Input Handlers

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

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (currentState != PanelState.Playing) return;
        if (pauseMenu != null)
            pauseMenu.Pause();
    }

    #endregion
}
