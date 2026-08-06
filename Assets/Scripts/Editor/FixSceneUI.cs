using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Idempotent, batch-safe editor tool that repairs the Florance scene UI:
///
///  * Deletes the duplicate PauseMenuManager component (deactivated first).
///  * Rewires every pause-menu button to the surviving PauseMenuManager.
///  * Creates and wires the HUD texts that were missing (timer, high score,
///    announcement).
///  * Replaces the HUD toys/bottles counters that were accidentally pointing at
///    the score popup objects with dedicated counter texts.
///  * Gives UIManager a dedicated end-screen "Final Score" text.
///  * Wires the InteractionPromptUI prompt/warning texts.
///
/// Existing authoring values and valid references are left untouched; the tool
/// only adds missing or wrong references.
/// </summary>
public static class FixSceneUI
{
    #region Constants

    private const string FloranceScenePath = "Assets/Scenes/Florance.unity";

    #endregion

    #region Public Methods

    /// <summary>Menu entry for interactive use inside the Editor.</summary>
    [MenuItem("Tools/Fix Scene UI")]
    public static void FixFromMenu()
    {
        Run();
    }

    /// <summary>
    /// Runs the full scene UI fix. Batch-safe: usable from the command line via
    /// <c>Unity.exe -batchmode -quit -projectPath &lt;path&gt; -executeMethod FixSceneUI.Run</c>.
    /// </summary>
    public static void Run()
    {
        bool interactive = !Application.isBatchMode;
        StringBuilder report = new StringBuilder();

        Scene scene = EditorSceneManager.OpenScene(FloranceScenePath, OpenSceneMode.Single);

        int removed = RemoveDuplicateManagers(report);

        // Reuse the established setup steps: they are no-ops for anything that is
        // already wired, so authoring values are preserved.
        AutoSetupPauseMenu.SetupButtons();
        AutoSetupPauseMenu.SetupHUDManager();
        AutoSetupPauseMenu.SetupInteractionPrompt();
        AutoSetupPauseMenu.SetupPauseMenuManager();

        int hudFixes = FixHUDConflictTexts(report);
        int uiFixes = FixUIManagerEndScreen(report);

        EditorSceneManager.SaveScene(scene);

        Debug.Log($"[FixSceneUI] removed={removed} hudFixes={hudFixes} uiFixes={uiFixes}\n{report}");

        if (interactive)
            EditorUtility.DisplayDialog("Fix Scene UI",
                $"Removed {removed} duplicate(s), applied {hudFixes + uiFixes} reference fix(es).\n\nSee the Console for the report.", "OK");
    }

    /// <summary>
    /// Re-checks the scene UI wiring, logging a report. Exits with code 0 when
    /// everything is wired correctly, 1 when issues remain.
    /// </summary>
    public static void Validate()
    {
        EditorSceneManager.OpenScene(FloranceScenePath, OpenSceneMode.Single);

        StringBuilder report = new StringBuilder();
        int problems = 0;

        PauseMenuManager[] pmm = Object.FindObjectsByType<PauseMenuManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (pmm.Length != 1)
        {
            problems++;
            report.AppendLine($"[FAIL] Expected exactly 1 PauseMenuManager, found {pmm.Length}.");
        }
        else
        {
            report.AppendLine("[OK ] Single PauseMenuManager present.");
        }

        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
        if (hud == null)
        {
            problems++;
            report.AppendLine("[FAIL] No HUDManager in scene.");
        }
        else
        {
            SerializedObject so = new SerializedObject(hud);
            foreach (string field in new[] { "collectedText", "toysText", "bottlesText", "livesText", "timerText", "scoreText", "highScoreText", "announcementText" })
            {
                if (so.FindProperty(field)?.objectReferenceValue == null)
                {
                    problems++;
                    report.AppendLine($"[FAIL] HUDManager.{field} is null.");
                }
            }
        }

        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui == null)
        {
            problems++;
            report.AppendLine("[FAIL] No UIManager in scene.");
        }
        else
        {
            SerializedObject so = new SerializedObject(ui);
            TMP_Text finalScore = so.FindProperty("finalScoreText")?.objectReferenceValue as TMP_Text;
            if (finalScore == null || !IsDescendantOf(finalScore.transform, "CreditsScreen"))
            {
                problems++;
                report.AppendLine("[FAIL] UIManager.finalScoreText is not wired to an end-screen text.");
            }
        }

        InteractionPromptUI prompt = Object.FindFirstObjectByType<InteractionPromptUI>();
        if (prompt == null)
        {
            problems++;
            report.AppendLine("[FAIL] No InteractionPromptUI in scene.");
        }
        else
        {
            SerializedObject so = new SerializedObject(prompt);
            if (so.FindProperty("promptText")?.objectReferenceValue == null)
            {
                problems++;
                report.AppendLine("[FAIL] InteractionPromptUI.promptText is null.");
            }
            if (so.FindProperty("warningText")?.objectReferenceValue == null)
            {
                problems++;
                report.AppendLine("[FAIL] InteractionPromptUI.warningText is null.");
            }
        }

        int unwired = 0;
        foreach (Button btn in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!IsButtonWired(btn, report))
            {
                unwired++;
                problems++;
            }
        }
        if (unwired == 0)
            report.AppendLine("[OK ] All buttons wired.");

        report.AppendLine(problems == 0
            ? "RESULT: Scene UI verified OK."
            : $"RESULT: {problems} issue(s) found.");

        Debug.Log("[FixSceneUI.Validate]\n" + report);

        if (Application.isBatchMode)
            EditorApplication.Exit(problems == 0 ? 0 : 1);
    }

    #endregion

    #region Scene Fixes

    private static int RemoveDuplicateManagers(StringBuilder report)
    {
        PauseMenuManager[] all = Object.FindObjectsByType<PauseMenuManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (all.Length <= 1)
        {
            report.AppendLine("[OK ] Single PauseMenuManager present.");
            return 0;
        }

        PauseMenuManager best = AutoSetupPauseMenu.FindBestPauseMenuManager();
        int removed = 0;
        foreach (PauseMenuManager m in all)
        {
            if (m == best) continue;

            GameObject go = m.gameObject;
            report.AppendLine($"[FIXED] Duplicate PauseMenuManager on '{go.name}' (parent: {(go.transform.parent != null ? go.transform.parent.name : "scene")}).");
            go.SetActive(false);
            Object.DestroyImmediate(go);
            removed++;
        }
        return removed;
    }

    private static int FixHUDConflictTexts(StringBuilder report)
    {
        HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
        Transform hudScreen = AutoSetupPauseMenu.FindTransformIncludingInactive("HUDScreen");
        if (hud == null || hudScreen == null) return 0;

        SerializedObject so = new SerializedObject(hud);
        int fixes = 0;

        if (TryFixPopupConflict(so, "toysText", "Toys Text", "Toys: 0", new Vector2(-460, -60), hudScreen, report)) fixes++;
        if (TryFixPopupConflict(so, "bottlesText", "Bottles Text", "Bottles: 0", new Vector2(-460, -100), hudScreen, report)) fixes++;

        so.ApplyModifiedProperties();
        return fixes;
    }

    private static bool TryFixPopupConflict(SerializedObject so, string field, string textName, string defaultText, Vector2 pos, Transform hudScreen, StringBuilder report)
    {
        SerializedProperty prop = so.FindProperty(field);
        TMP_Text current = prop?.objectReferenceValue as TMP_Text;
        if (current == null) return false;

        // A counter must not share its target with a score popup object.
        GameObject popup = so.FindProperty(field == "toysText" ? "toyScorePopup" : "plasticBottleScorePopup")?.objectReferenceValue as GameObject;
        bool conflict = popup != null && current.gameObject == popup;
        conflict |= current.gameObject.name == "PlantScore" || current.gameObject.name == "ToyScore" || current.gameObject.name == "PlasticBottleScore";

        if (!conflict) return false;

        report.AppendLine($"[FIXED] HUDManager.{field} pointed at popup '{current.gameObject.name}'; assigned dedicated '{textName}'.");
        prop.objectReferenceValue = AutoSetupPauseMenu.FindOrCreateTMPText(hudScreen, textName, defaultText, pos, true);
        return true;
    }

    private static int FixUIManagerEndScreen(StringBuilder report)
    {
        UIManager ui = Object.FindFirstObjectByType<UIManager>();
        if (ui == null) return 0;

        SerializedObject so = new SerializedObject(ui);
        SerializedProperty finalScore = so.FindProperty("finalScoreText");
        TMP_Text current = finalScore?.objectReferenceValue as TMP_Text;

        if (current != null && IsDescendantOf(current.transform, "CreditsScreen"))
            return 0;

        GameObject endPanel = so.FindProperty("endPanel")?.objectReferenceValue as GameObject;
        Transform endTransform = endPanel != null ? endPanel.transform : AutoSetupPauseMenu.FindTransformIncludingInactive("CreditsScreen");
        if (endTransform == null) return 0;

        TMP_Text text = AutoSetupPauseMenu.FindOrCreateTMPText(endTransform, "FinalScoreText", "Final Score: 0", new Vector2(0, 60), true);
        finalScore.objectReferenceValue = text;
        so.ApplyModifiedProperties();
        report.AppendLine($"[FIXED] UIManager.finalScoreText -> '{text.gameObject.name}' under {endTransform.name}.");
        return 1;
    }

    #endregion

    #region Validation Helpers

    private static bool IsButtonWired(Button button, StringBuilder report)
    {
        SerializedObject so = new SerializedObject(button);
        SerializedProperty calls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
        if (calls == null || calls.arraySize == 0)
        {
            report.AppendLine($"[FAIL] Button '{button.gameObject.name}' has no OnClick calls.");
            return false;
        }

        SerializedProperty call = calls.GetArrayElementAtIndex(0);
        Object target = call.FindPropertyRelative("m_Target").objectReferenceValue;
        string method = call.FindPropertyRelative("m_MethodName").stringValue;

        if (target == null || string.IsNullOrEmpty(method))
        {
            report.AppendLine($"[FAIL] Button '{button.gameObject.name}' has an empty onClick target/method.");
            return false;
        }

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        if (target.GetType().GetMethod(method, flags) == null)
        {
            report.AppendLine($"[FAIL] Button '{button.gameObject.name}' references missing method '{method}' on {target.GetType().Name}.");
            return false;
        }

        return true;
    }

    private static bool IsDescendantOf(Transform t, string ancestorName)
    {
        while (t != null)
        {
            if (t.name == ancestorName) return true;
            t = t.parent;
        }
        return false;
    }

    #endregion
}
