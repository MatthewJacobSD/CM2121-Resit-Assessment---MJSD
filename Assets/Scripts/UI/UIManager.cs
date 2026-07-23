using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    private InputAction cancelAction;
    private InputAction navigateAction;
    private InputAction pauseAction;
    private InputAction restartAction;
    private InputAction submitAction;
    private InputAction continueAction;

    private enum PanelState
    {
        Welcome,
        Instructions,
        Playing,
        Ended
    }

    private PanelState currentState;


    private void Awake()
    {
        var UIMap = playerControls.FindActionMap("UI", true);

        cancelAction = UIMap.FindAction("Cancel", true);
        navigateAction = UIMap.FindAction("Navigate", true);
        pauseAction = UIMap.FindAction("Pause", true);
        restartAction = UIMap.FindAction("Restart", true);
        submitAction = UIMap.FindAction("Submit", true);
        continueAction = UIMap.FindAction("Continue", true);
    }


    private void Start()
    {
        if (canvasRoot)
            canvasRoot.SetActive(true);

        ShowPanel(PanelState.Welcome);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver += ShowEndScreen;
    }


    private void OnEnable()
    {
        cancelAction.Enable();
        navigateAction.Enable();
        pauseAction.Enable();
        restartAction.Enable();
        submitAction.Enable();
        continueAction.Enable();

        continueAction.performed += OnContinue;
    }


    private void OnDisable()
    {
        cancelAction.Disable();
        navigateAction.Disable();
        pauseAction.Disable();
        restartAction.Disable();
        submitAction.Disable();
        continueAction.Disable();

        continueAction.performed -= OnContinue;

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowEndScreen;
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

                if (GameManager.Instance != null)
                    GameManager.Instance.StartGame();

                break;
        }
    }


    private void ShowPanel(PanelState state)
    {
        currentState = state;

        if (welcomePanel)
            welcomePanel.SetActive(state == PanelState.Welcome);

        if (instructionPanel)
            instructionPanel.SetActive(state == PanelState.Instructions);

        if (hudPanel)
            hudPanel.SetActive(state == PanelState.Playing);

        if (endPanel)
            endPanel.SetActive(state == PanelState.Ended);


        UpdateCursorState(state);
    }


    private void UpdateCursorState(PanelState state)
    {
        switch (state)
        {
            case PanelState.Playing:

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                break;


            case PanelState.Welcome:
            case PanelState.Instructions:
            case PanelState.Ended:

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                break;
        }
    }


    private void OnContinue(InputAction.CallbackContext ctx)
    {
        if (currentState == PanelState.Welcome ||
            currentState == PanelState.Instructions)
        {
            AdvancePanel();
        }
    }


    private void ShowEndScreen()
    {
        ShowPanel(PanelState.Ended);


        if (ScoreManager.Instance == null ||
            GameManager.Instance == null)
            return;


        int score = ScoreManager.Instance.CurrentScore;


        if (finalScoreText)
            finalScoreText.text = $"Final Score: {score}";


        if (perfectCondition)
            perfectCondition.SetActive(false);

        if (defaultCondition)
            defaultCondition.SetActive(false);

        if (failureCondition)
            failureCondition.SetActive(false);



        if (score >= 30)
        {
            if (perfectCondition)
                perfectCondition.SetActive(true);

            if (congratulationText)
                congratulationText.text = "Perfect Cleanup!";
        }
        else if (score > 0)
        {
            if (defaultCondition)
                defaultCondition.SetActive(true);

            if (congratulationText)
                congratulationText.text = "Good Effort!";
        }
        else
        {
            if (failureCondition)
                failureCondition.SetActive(true);

            if (congratulationText)
                congratulationText.text = "Try Again!";
        }
    }


    public void OnRestart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();

        ShowPanel(PanelState.Playing);
    }
}