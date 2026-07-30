using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmExitPanel;
    [SerializeField] private GameObject saveProgressPanel;

    [Header("Settings")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Slider volumeSlider;

    [Header("Pause Display")]
    [SerializeField] private TMP_Text currentScoreText;

    private UIManager uiManagers;
    private bool isPaused;
    private bool isGameSaved;

    private const string UsernameKey = "Username";
    private const string VolumeKey = "Volume";

    private void Start()
    {
        uiManagers = FindFirstObjectByType<UIManager>();

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        HideAll();
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    #region Pause / Resume

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;
        GameManager.Instance.PauseGame();

        HideAll();
        pausePanel.SetActive(true);

        if (currentScoreText != null && ScoreManager.Instance != null)
            currentScoreText.text = $"Score: {ScoreManager.Instance.CurrentScore}";

        if (uiManagers != null)
            uiManagers.ShowPauseMenu();
    }

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

    public void SaveAndQuit()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveHighScore();

        isGameSaved = true;
        PlayerPrefs.Save();

        isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.ResumeGame();
        GameManager.Instance.RestartGame();
    }

    public void QuitWithoutSaving()
    {
        isPaused = false;
        Time.timeScale = 1f;
        GameManager.Instance.ResumeGame();
        GameManager.Instance.RestartGame();
    }

    public void SaveProgress_Cancel()
    {
        BackToPauseMenu();
    }

    #endregion

    #region Utility

    private void HideAll()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (confirmExitPanel != null) confirmExitPanel.SetActive(false);
        if (saveProgressPanel != null) saveProgressPanel.SetActive(false);
    }

    #endregion
}
