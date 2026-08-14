using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns the in-game pause menu: pausing/resuming, settings (username and volume),
/// a two-step exit confirmation, and score persistence on quit.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    #region Constants

    private const string UsernameKey = "Username";
    private const string VolumeKey = "Volume";

    #endregion

    #region Serialized Fields

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private GameObject saveProgressPanel;

    [Header("Settings")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Slider volumeSlider;

    [Header("Pause Display")]
    [Tooltip("Optional label showing the current score on the pause screen.")]
    [SerializeField] private TMP_Text currentScoreText;

    #endregion

    #region Private Fields

    private UIManager uiManagers;
    private bool isPaused;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        uiManagers = FindFirstObjectByType<UIManager>();

        // Apply saved volume on load.
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        HideAll();
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    #endregion

    #region Pause / Resume

    /// <summary>Pauses gameplay, freezes time and shows the pause panel.</summary>
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;
        GameManager.Instance.PauseGame();

        HideAll();
        pausePanel.SetActive(true);

        if (currentScoreText != null && ScoreManager.Instance != null)
            currentScoreText.text = ScoreManager.Instance.CurrentScore.ToString();

        if (uiManagers != null)
            uiManagers.ShowPauseMenu();
    }

    /// <summary>Resumes gameplay, unfreezes time and hides the pause panel.</summary>
    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        HideAll();
        Time.timeScale = 1f;
        GameManager.Instance.ResumeGame();

        if (uiManagers != null)
            uiManagers.ReturnFromPause();
    }

    #endregion

    #region Pause Menu Buttons

    public void OnContinuePressed()
    {
        Resume();
    }

    public void OnSettingsPressed()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        LoadSettings();
    }

    public void OnExitPressed()
    {
        pausePanel.SetActive(false);
        confirmExitPanel.SetActive(true);
    }

    public void OnRestartPressed()
    {
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.RestartGame();
    }

    #endregion

    #region Settings

    private void LoadSettings()
    {
        if (usernameInput != null)
            usernameInput.text = PlayerPrefs.GetString(UsernameKey, "");

        if (volumeSlider != null)
            volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey, 1f);
    }

    public void OnSaveUsername()
    {
        if (usernameInput != null)
            PlayerPrefs.SetString(UsernameKey, usernameInput.text);

        PlayerPrefs.Save();
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    public void OnSaveSettings()
    {
        OnSaveUsername();

        if (volumeSlider != null)
            PlayerPrefs.SetFloat(VolumeKey, volumeSlider.value);

        PlayerPrefs.Save();
        BackToPauseMenu();
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);
        saveProgressPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    #endregion

    #region Exit Flow

    public void ConfirmExit_Yes()
    {
        confirmExitPanel.SetActive(false);
        saveProgressPanel.SetActive(true);
    }

    public void ConfirmExit_No()
    {
        BackToPauseMenu();
    }

    /// <summary>Saves the high score, then quits back to the title screen.</summary>
    public void SaveAndQuit()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();

        PlayerPrefs.Save();

        ExitToTitle();
    }

    /// <summary>Quits to the title screen without persisting the current score.</summary>
    public void QuitWithoutSaving()
    {
        ExitToTitle();
    }

    public void SaveProgress_Cancel()
    {
        BackToPauseMenu();
    }

    #endregion

    #region Utility

    // Restarting the scene acts as "quit to title" because the game starts on the
    // welcome screen and the score was already persisted above when required.
    private void ExitToTitle()
    {
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.ResumeGame();
        GameManager.Instance.RestartGame();
    }

    private void HideAll()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (confirmExitPanel != null) confirmExitPanel.SetActive(false);
        if (saveProgressPanel != null) saveProgressPanel.SetActive(false);
    }

    #endregion
}
