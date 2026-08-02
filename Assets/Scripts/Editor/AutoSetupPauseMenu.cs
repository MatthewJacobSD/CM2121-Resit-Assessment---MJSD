using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Editor tool that builds the full UI hierarchy (canvas, panels, pause menu,
/// buttons, input wiring) in the active scene with one menu click.
/// </summary>
public static class AutoSetupPauseMenu
{
    #region Public Methods

    /// <summary>Menu entry: sets up the complete UI in the currently open scene.</summary>
    [MenuItem("Tools/Setup UI in Current Scene")]
    public static void Setup()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;

        Undo.SetCurrentGroupName("Setup UI - " + sceneName);
        int group = Undo.GetCurrentGroup();

        SetupCanvasStructure();
        SetupUIManager();
        SetupPauseMenuManager();
        SetupButtons();
        SetupHUDManager();

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);

        EditorUtility.DisplayDialog("Setup Complete",
            "UI setup finished for: " + sceneName + "\n\n" +
            "Done:\n" +
            "- Canvas panels wired to UIManager\n" +
            "- PauseMenu hierarchy created\n" +
            "- PauseMenuManager added with references\n" +
            "- Buttons rewired\n" +
            "- Input System references updated\n\n" +
            "Still needed:\n" +
            "1. Check HUDManager references in Inspector\n" +
            "2. Add VolumeSlider to SettingsPanel\n" +
            "3. Wire volumeSlider to PauseMenuManager", "OK");
    }

    #endregion

    #region Setup Steps

    private static void SetupCanvasStructure()
    {
        // Find or create the Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
        }

        // Find or create EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(InputSystem.UI.InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(esGO, "Create EventSystem");
        }

        // Find PanelUI or create it
        GameObject panelUI = GameObject.Find("PanelUI");
        if (panelUI == null)
        {
            panelUI = new GameObject("PanelUI", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(panelUI, "Create PanelUI");
            panelUI.transform.SetParent(canvas.transform, false);
            panelUI.layer = 5;
        }

        // Ensure the four main panels exist
        EnsurePanel(panelUI.transform, "welcomePanel", "WelcomeScreen");
        EnsurePanel(panelUI.transform, "instructionPanel", "InstructionScreen");
        EnsurePanel(panelUI.transform, "hudPanel", "HUDScreen");
        EnsurePanel(panelUI.transform, "endPanel", "CreditsScreen", "EndScreen");
    }

    private static void EnsurePanel(Transform parent, params string[] names)
    {
        foreach (string name in names)
        {
            Transform existing = FindChildRecursive(parent, name);
            if (existing != null) return;
        }

        // Create it using the first name
        GameObject panel = new GameObject(names[0], typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panel, "Create " + names[0]);
        panel.transform.SetParent(parent, false);
        panel.layer = 5;
        panel.SetActive(false);
    }

    private static void SetupUIManager()
    {
        UIManager uiMgrs = Object.FindFirstObjectByType<UIManager>();
        if (uiMgrs == null)
        {
            GameObject uiMgrGO = new GameObject("UIManager");
            Undo.RegisterCreatedObjectUndo(uiMgrGO, "Create UIManager");
            uiMgrs = uiMgrGO.AddComponent<UIManager>();
        }

        SerializedObject so = new SerializedObject(uiMgrs);

        // Find panels by name
        Transform panelUI = GameObject.Find("PanelUI")?.transform;
        if (panelUI != null)
        {
            so.FindProperty("welcomePanel").objectReferenceValue = FindChildRecursive(panelUI, "WelcomeScreen")?.gameObject;
            so.FindProperty("instructionPanel").objectReferenceValue = FindChildRecursive(panelUI, "InstructionScreen")?.gameObject;
            so.FindProperty("hudPanel").objectReferenceValue = FindChildRecursive(panelUI, "HUDScreen")?.gameObject;
            so.FindProperty("endPanel").objectReferenceValue = FindChildRecursive(panelUI, "CreditsScreen")?.gameObject ?? FindChildRecursive(panelUI, "EndScreen")?.gameObject;
        }

        // Find InputActionAsset
        string[] guids = AssetDatabase.FindAssets("t:InputActionAsset");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
            if (asset != null && asset.name.Contains("ActionsControl"))
            {
                so.FindProperty("playerControls").objectReferenceValue = asset;
                break;
            }
        }

        so.ApplyModifiedProperties();
    }

    private static void SetupPauseMenuManager()
    {
        // Find Canvas and PanelUI
        Transform canvas = Object.FindFirstObjectByType<Canvas>()?.transform;
        Transform panelUI = GameObject.Find("PanelUI")?.transform;
        if (canvas == null || panelUI == null) return;

        // Find or create PauseMenu as sibling of PanelUI (under Canvas)
        Transform pauseMenuTransform = FindChildRecursive(canvas, "PauseMenu");
        GameObject pauseMenuGO;
        if (pauseMenuTransform != null)
            pauseMenuGO = pauseMenuTransform.gameObject;
        else
        {
            pauseMenuGO = new GameObject("PauseMenu", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(pauseMenuGO, "Create PauseMenu");
            pauseMenuGO.transform.SetParent(canvas, false);
            pauseMenuGO.layer = 5;
            pauseMenuGO.SetActive(false);
        }

        PauseMenuManager mgr = Object.FindFirstObjectByType<PauseMenuManager>();
        if (mgr == null)
        {
            GameObject mgrGO = new GameObject("PauseMenuManager");
            Undo.RegisterCreatedObjectUndo(mgrGO, "Create PauseMenuManager");
            mgr = mgrGO.AddComponent<PauseMenuManager>();
        }

        SerializedObject so = new SerializedObject(mgr);

        // Create or find PausePanel
        Transform pausePanel = FindChildRecursive(pauseMenuGO.transform, "PausePanel");
        if (pausePanel == null)
            pausePanel = CreateUIPanel(pauseMenuGO.transform, "PausePanel", new Vector2(400, 500));
        so.FindProperty("pausePanel").objectReferenceValue = pausePanel?.gameObject;

        // Create or find SettingsPanel
        Transform settingsPanel = FindChildRecursive(pauseMenuGO.transform, "SettingsPanel");
        if (settingsPanel == null)
            settingsPanel = CreateUIPanel(pauseMenuGO.transform, "SettingsPanel", new Vector2(400, 500));
        so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel?.gameObject;

        // Create or find ConfirmationModal
        Transform confirmModal = FindChildRecursive(pauseMenuGO.transform, "ConfirmationModal");
        if (confirmModal == null)
            confirmModal = CreateUIPanel(pauseMenuGO.transform, "ConfirmationModal", new Vector2(400, 200));
        so.FindProperty("confirmExitPanel").objectReferenceValue = confirmModal?.gameObject;

        // Create or find SaveProgressModal
        Transform saveModal = FindChildRecursive(pauseMenuGO.transform, "SaveProgressModal");
        if (saveModal == null)
            saveModal = CreateUIPanel(pauseMenuGO.transform, "SaveProgressModal", new Vector2(400, 250));
        so.FindProperty("saveProgressPanel").objectReferenceValue = saveModal?.gameObject;

        // Try to find username input in settings
        if (settingsPanel != null)
        {
            TMP_InputField inputField = settingsPanel.GetComponentInChildren<TMP_InputField>(true);
            if (inputField != null)
                so.FindProperty("usernameInput").objectReferenceValue = inputField;
        }

        so.ApplyModifiedProperties();

        // Set PauseMenu as inactive by default
        pauseMenuGO.SetActive(false);
    }

    private static Transform CreateUIPanel(Transform parent, string name, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);
        go.layer = 5;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);

        // Create a Context child for layout
        GameObject context = new GameObject("Context", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(context, "Create Context");
        context.transform.SetParent(go.transform, false);
        context.layer = 5;

        RectTransform ctxRT = context.GetComponent<RectTransform>();
        ctxRT.anchorMin = Vector2.zero;
        ctxRT.anchorMax = Vector2.one;
        ctxRT.sizeDelta = Vector2.zero;
        ctxRT.offsetMin = new Vector2(20, 20);
        ctxRT.offsetMax = new Vector2(-20, -20);

        VerticalLayoutGroup vlg = context.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.spacing = 10;

        return go.transform;
    }

    private static void SetupHUDManager()
    {
        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
        if (hud == null) return;

        SerializedObject so = new SerializedObject(hud);

        // Find TMP texts in the HUDScreen
        Transform hudScreen = GameObject.Find("HUDScreen")?.transform;
        if (hudScreen == null) return;

        // Create default text elements if they're missing
        TMP_Text[] texts = hudScreen.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text t in texts)
        {
            string name = t.gameObject.name.ToLower();
            if (name.Contains("lives") && so.FindProperty("livesText").objectReferenceValue == null)
                so.FindProperty("livesText").objectReferenceValue = t;
            else if (name.Contains("timer") && so.FindProperty("timerText").objectReferenceValue == null)
                so.FindProperty("timerText").objectReferenceValue = t;
            else if (name.Contains("score") && so.FindProperty("scoreText").objectReferenceValue == null)
                so.FindProperty("scoreText").objectReferenceValue = t;
            else if (name.Contains("high") && so.FindProperty("highScoreText").objectReferenceValue == null)
                so.FindProperty("highScoreText").objectReferenceValue = t;
            else if (name.Contains("announcement") && so.FindProperty("announcementText").objectReferenceValue == null)
                so.FindProperty("announcementText").objectReferenceValue = t;
            else if (name.Contains("plant") && so.FindProperty("collectedText").objectReferenceValue == null)
                so.FindProperty("collectedText").objectReferenceValue = t;
        }

        so.ApplyModifiedProperties();
    }

    private static void SetupButtons()
    {
        PauseMenuManager mgr = Object.FindFirstObjectByType<PauseMenuManager>();
        if (mgr == null) return;

        foreach (Button btn in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string name = btn.gameObject.name.ToLower();
            string parent = btn.transform.parent?.name.ToLower() ?? "";
            string grand = btn.transform.parent?.parent?.name.ToLower() ?? "";
            string great = btn.transform.parent?.parent?.parent?.name.ToLower() ?? "";

            if (name.Contains("continue"))
                WireButton(btn, mgr, "OnContinuePressed");
            else if (name.Contains("settings"))
                WireButton(btn, mgr, "OnSettingsPressed");
            else if (name == "updatebutton" || (name.Contains("update") && (parent.Contains("setting") || grand.Contains("setting"))))
                WireButton(btn, mgr, "OnSaveUsername");
            else if (name.Contains("back") || name.Contains("return") || name == "goback")
                WireButton(btn, mgr, "BackToPauseMenu");
            else if (name.Contains("exit") || name.Contains("quit"))
                WireButton(btn, mgr, "OnExitPressed");
            else if (name.Contains("restart"))
                WireButton(btn, mgr, "OnRestartPressed");
            else if (name == "yes" || name == "yesbutton")
            {
                if (grand.Contains("save") || great.Contains("save"))
                    WireButton(btn, mgr, "SaveAndQuit");
                else
                    WireButton(btn, mgr, "ConfirmExit_Yes");
            }
            else if (name == "no" || name == "nobutton")
            {
                if (grand.Contains("save") || great.Contains("save"))
                    WireButton(btn, mgr, "QuitWithoutSaving");
                else
                    WireButton(btn, mgr, "ConfirmExit_No");
            }
            else if (name.Contains("cancel"))
                WireButton(btn, mgr, "SaveProgress_Cancel");
        }
    }

    #endregion

    #region Utility

    private static void WireButton(Button button, PauseMenuManager target, string methodName)
    {
        SerializedObject so = new SerializedObject(button);
        SerializedProperty onClick = so.FindProperty("m_OnClick");
        if (onClick == null) return;

        onClick.ClearArray();
        onClick.arraySize = 1;

        SerializedProperty call = onClick.GetArrayElementAtIndex(0);
        call.FindPropertyRelative("m_Target").objectReferenceValue = target;
        call.FindPropertyRelative("m_MethodName").stringValue = methodName;
        call.FindPropertyRelative("m_CallState").enumValueIndex = 2;

        so.ApplyModifiedProperties();
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent == null) return null;
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    #endregion
}
