using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;

public static class BuildScript
{
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            UnityEngine.Debug.LogError("No scenes enabled in Build Settings.");
            return;
        }

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Build/CM2121_EcoRescue.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        UnityEngine.Debug.Log($"Build {(summary.result == BuildResult.Succeeded ? "SUCCEEDED" : "FAILED")}: {summary.totalSize} bytes, {summary.totalErrors} errors, {summary.totalWarnings} warnings");

        if (summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
        else
            EditorApplication.Exit(0);
    }
}
