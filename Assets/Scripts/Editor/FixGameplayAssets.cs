using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Idempotent, Undo-aware editor tool that fixes the serialized gameplay asset
/// data of the FloranceOverflow project:
///
///  * Adds a root BoxCollider to every collectible and bin prefab that lacks
///    one (fitted to the model mesh bounds).
///  * Sets collectible roots to the "Interactable" layer and bin roots to the
///    "Bin" layer via LayerMask.NameToLayer (never hardcoded ints).
///  * Marks bin colliders as triggers.
///  * Corrects the Shiba plushie item type (Plant -&gt; Toy) and display name.
///  * Corrects the "PlasticBott;e" item-name typo.
///  * Updates the GameManager scene instance required counts to 12 / 8 / 4.
///
/// The tool deliberately does NOT: create placeholder terrain, reposition scene
/// objects, or alter level design. It only touches serialized component data.
/// </summary>
public static class FixGameplayAssets
{
    #region Constants

    private const string FloranceScenePath = "Assets/Scenes/Florance.unity";

    private const string InteractableLayerName = "Interactable";
    private const string BinLayerName = "Bin";

    private const string ShibaDisplayName = "Shiba Plushie";
    private const string PlasticBottleDisplayName = "Plastic Bottle";
    private const string BottleNameTypo = "PlasticBott;e";

    // Objectives verified against the scene inventory (12 plants / 8 toys / 4 bottles).
    private const int PlantsRequired = 12;
    private const int ToysRequired = 8;
    private const int BottlesRequired = 4;

    #endregion

    #region Public Methods

    /// <summary>Menu entry for interactive use inside the Editor.</summary>
    [MenuItem("Tools/Fix Gameplay Assets")]
    public static void FixFromMenu()
    {
        Run();
    }

    /// <summary>
    /// Runs the full fix. Batch-safe: usable from the command line via
    /// <c>Unity.exe -batchmode -quit -projectPath &lt;path&gt; -executeMethod FixGameplayAssets.Run</c>.
    /// </summary>
    public static void Run()
    {
        bool interactive = !Application.isBatchMode;
        int undoGroup = 0;
        if (interactive)
        {
            undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Fix Gameplay Assets");
        }

        StringBuilder report = new StringBuilder();
        int changed = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Models/Prefabs" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("/Collectables/"))
            {
                if (FixCollectiblePrefab(path, interactive, report)) changed++;
            }
            else if (path.Contains("/Bins/"))
            {
                if (FixBinPrefab(path, interactive, report)) changed++;
            }
        }

        FixGameManagerCounts(report);

        if (interactive)
            Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[FixGameplayAssets] {changed} prefab(s) changed. Report:\n{report}");

        if (interactive)
            EditorUtility.DisplayDialog("Fix Gameplay Assets",
                $"Changed {changed} prefab(s).\n\nSee the Console for the per-prefab report.", "OK");
    }

    /// <summary>
    /// Re-checks every collectible/bin prefab and the GameManager scene counts,
    /// logging a per-prefab status report. Exits with code 0 when everything is
    /// already correct, 1 when items still need attention.
    /// </summary>
    public static void Validate()
    {
        StringBuilder report = new StringBuilder();
        int problems = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Models/Prefabs" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("/Collectables/") || path.Contains("/Bins/"))
            {
                if (!VerifyPrefab(path, report)) problems++;
            }
        }

        int targetLayer = LayerMask.NameToLayer(InteractableLayerName);
        int binLayer = LayerMask.NameToLayer(BinLayerName);
        if (targetLayer < 0) { problems++; report.AppendLine("[FAIL] Missing layer \"" + InteractableLayerName + "\" in TagManager."); }
        if (binLayer < 0) { problems++; report.AppendLine("[FAIL] Missing layer \"" + BinLayerName + "\" in TagManager."); }

        if (!VerifyGameManagerCounts(report)) problems++;

        report.AppendLine(problems == 0
            ? "RESULT: All gameplay assets verified OK."
            : $"RESULT: {problems} issue(s) found.");

        Debug.Log("[FixGameplayAssets.Validate]\n" + report);

        if (Application.isBatchMode)
            EditorApplication.Exit(problems == 0 ? 0 : 1);
    }

    #endregion

    #region Prefab Fixes

    private static bool FixCollectiblePrefab(string path, bool interactive, StringBuilder report)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            report.AppendLine($"[SKIP] Cannot load: {path}");
            return false;
        }

        List<string> changes = new List<string>();

        EnsureCollider(prefab, false, interactive, changes);
        EnsureLayer(prefab, InteractableLayerName, interactive, changes);

        PickupItem pickup = prefab.GetComponent<PickupItem>();
        if (pickup == null)
        {
            report.AppendLine($"[WARN] {path}: no PickupItem component found.");
        }
        else
        {
            bool isShiba = prefab.name.Contains("Shiba");
            bool isBottle = path.Contains("/Bottle/");

            if (isShiba && pickup.ItemType != ItemType.Toy)
            {
                SetItemType(pickup, ItemType.Toy, interactive);
                changes.Add("itemType Plant -> Toy");
            }

            if (isShiba && pickup.ItemName != ShibaDisplayName)
            {
                SetItemName(pickup, ShibaDisplayName, interactive);
                changes.Add("itemName -> \"Shiba Plushie\"");
            }

            if (isBottle && pickup.ItemName == BottleNameTypo)
            {
                SetItemName(pickup, PlasticBottleDisplayName, interactive);
                changes.Add("itemName \"PlasticBott;e\" -> \"Plastic Bottle\"");
            }
        }

        return FinishPrefab(path, prefab, changes, report);
    }

    private static bool FixBinPrefab(string path, bool interactive, StringBuilder report)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            report.AppendLine($"[SKIP] Cannot load: {path}");
            return false;
        }

        List<string> changes = new List<string>();

        EnsureCollider(prefab, true, interactive, changes);
        EnsureLayer(prefab, BinLayerName, interactive, changes);

        return FinishPrefab(path, prefab, changes, report);
    }

    private static bool FinishPrefab(string path, GameObject prefab, List<string> changes, StringBuilder report)
    {
        if (changes.Count == 0)
        {
            report.AppendLine($"[OK ] {path}");
            return false;
        }

        PrefabUtility.SavePrefabAsset(prefab);
        AssetDatabase.SaveAssets();
        report.AppendLine($"[FIXED] {path}");
        report.AppendLine("         - " + string.Join("\n         - ", changes));
        return true;
    }

    #endregion

    #region Single-Asset Helpers

    private static void EnsureCollider(GameObject prefab, bool isTrigger, bool interactive, List<string> changes)
    {
        if (prefab.GetComponent<Collider>() != null)
        {
            Collider existing = prefab.GetComponent<Collider>();
            if (existing is BoxCollider box && box.isTrigger != isTrigger)
            {
                if (interactive) Undo.RecordObject(box, "Set trigger");
                box.isTrigger = isTrigger;
                changes.Add(isTrigger ? "trigger enabled" : "trigger disabled");
            }
            return;
        }

        if (interactive) Undo.RegisterCreatedObjectUndo(prefab, "Add BoxCollider");
        BoxCollider collider = prefab.AddComponent<BoxCollider>();
        collider.isTrigger = isTrigger;
        FitColliderToMesh(collider, prefab);
        changes.Add($"BoxCollider center={collider.center} size={collider.size}{(isTrigger ? " (trigger)" : "")}");
    }

    private static void EnsureLayer(GameObject prefab, string layerName, bool interactive, List<string> changes)
    {
        int target = LayerMask.NameToLayer(layerName);
        if (target < 0)
        {
            Debug.LogError($"[FixGameplayAssets] Layer \"{layerName}\" is not defined in TagManager. Skipping.");
            return;
        }

        if (prefab.layer == target) return;

        if (interactive) Undo.RecordObject(prefab, "Set layer");
        prefab.layer = target;
        changes.Add($"layer -> \"{layerName}\" ({target})");
    }

    private static void FitColliderToMesh(BoxCollider box, GameObject root)
    {
        bool found = false;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (MeshFilter mf in root.GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh == null) continue;

            Vector3 c = mf.sharedMesh.bounds.center;
            Vector3 e = mf.sharedMesh.bounds.extents;

            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = c + new Vector3(
                    e.x * ((i & 1) != 0 ? 1f : -1f),
                    e.y * ((i & 2) != 0 ? 1f : -1f),
                    e.z * ((i & 4) != 0 ? 1f : -1f));

                Vector3 local = root.transform.InverseTransformPoint(mf.transform.TransformPoint(corner));
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            found = true;
        }

        if (found)
        {
            box.center = (min + max) * 0.5f;
            box.size = max - min;
        }
        // No mesh found: keep the default 1x1x1 box so the object is still
        // physically present; the report shows the size so it can be refined.
    }

    private static void SetItemType(PickupItem pickup, ItemType type, bool interactive)
    {
        if (interactive) Undo.RecordObject(pickup, "Set item type");
        SerializedObject so = new SerializedObject(pickup);
        SerializedProperty prop = so.FindProperty("itemType");
        if (prop != null)
        {
            prop.enumValueIndex = (int)type;
            so.ApplyModifiedProperties();
        }
    }

    private static void SetItemName(PickupItem pickup, string name, bool interactive)
    {
        if (interactive) Undo.RecordObject(pickup, "Set item name");
        SerializedObject so = new SerializedObject(pickup);
        SerializedProperty prop = so.FindProperty("itemName");
        if (prop != null)
        {
            prop.stringValue = name;
            so.ApplyModifiedProperties();
        }
    }

    #endregion

    #region GameManager Counts

    private static void FixGameManagerCounts(StringBuilder report)
    {
        Scene scene = EditorSceneManager.OpenScene(FloranceScenePath, OpenSceneMode.Single);

        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            report.AppendLine("[WARN] GameManager not found in " + FloranceScenePath);
            return;
        }

        SerializedObject so = new SerializedObject(gameManager);
        List<string> changes = new List<string>();
        SetRequiredCount(so, "plantsRequired", PlantsRequired, changes);
        SetRequiredCount(so, "toysRequired", ToysRequired, changes);
        SetRequiredCount(so, "bottlesRequired", BottlesRequired, changes);
        so.ApplyModifiedProperties();

        if (changes.Count > 0)
        {
            EditorSceneManager.SaveScene(scene);
            report.AppendLine("[FIXED] GameManager counts: " + string.Join(", ", changes));
        }
        else
        {
            report.AppendLine("[OK ] GameManager counts already " + PlantsRequired + "/" + ToysRequired + "/" + BottlesRequired + ".");
        }
    }

    private static void SetRequiredCount(SerializedObject so, string fieldName, int value, List<string> changes)
    {
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            // Field does not exist yet (e.g. tool run before GameManager phase).
            Debug.LogWarning($"[FixGameplayAssets] GameManager field \"{fieldName}\" not found; skipping.");
            return;
        }

        if (prop.intValue != value)
        {
            prop.intValue = value;
            changes.Add($"{fieldName}: {prop.intValue} -> {value}");
        }
    }

    #endregion

    #region Validation

    private static bool VerifyGameManagerCounts(StringBuilder report)
    {
        Scene scene = EditorSceneManager.OpenScene(FloranceScenePath, OpenSceneMode.Single);

        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            report.AppendLine("[FAIL] GameManager not found in " + FloranceScenePath);
            return false;
        }

        SerializedObject so = new SerializedObject(gameManager);
        bool ok = true;
        VerifyCount(so, "plantsRequired", PlantsRequired, report, ref ok);
        VerifyCount(so, "toysRequired", ToysRequired, report, ref ok);
        VerifyCount(so, "bottlesRequired", BottlesRequired, report, ref ok);

        if (ok)
            report.AppendLine("[OK ] GameManager counts are " + PlantsRequired + "/" + ToysRequired + "/" + BottlesRequired + ".");
        return ok;
    }

    private static void VerifyCount(SerializedObject so, string fieldName, int expected, StringBuilder report, ref bool ok)
    {
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            report.AppendLine($"[FAIL] GameManager field \"{fieldName}\" missing.");
            ok = false;
            return;
        }

        if (prop.intValue != expected)
        {
            report.AppendLine($"[FAIL] GameManager {fieldName} is {prop.intValue}, expected {expected}.");
            ok = false;
        }
    }

    private static bool VerifyPrefab(string path, StringBuilder report)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            report.AppendLine($"[FAIL] Cannot load: {path}");
            return false;
        }

        bool ok = true;
        bool isBin = path.Contains("/Bins/");
        string expectedLayer = isBin ? BinLayerName : InteractableLayerName;
        int expectedLayerIndex = LayerMask.NameToLayer(expectedLayer);

        if (prefab.GetComponent<Collider>() == null)
        {
            report.AppendLine($"[FAIL] {path}: missing collider");
            ok = false;
        }

        if (expectedLayerIndex >= 0 && prefab.layer != expectedLayerIndex)
        {
            report.AppendLine($"[FAIL] {path}: layer is {prefab.layer}, expected {expectedLayer} ({expectedLayerIndex})");
            ok = false;
        }

        if (!isBin)
        {
            PickupItem pickup = prefab.GetComponent<PickupItem>();
            if (pickup == null)
            {
                report.AppendLine($"[FAIL] {path}: missing PickupItem");
                ok = false;
            }
            else if (prefab.name.Contains("Shiba") && pickup.ItemType != ItemType.Toy)
            {
                report.AppendLine($"[FAIL] {path}: Shiba itemType is {pickup.ItemType}, expected Toy");
                ok = false;
            }
            else if (path.Contains("/Bottle/") && pickup.ItemName == BottleNameTypo)
            {
                report.AppendLine($"[FAIL] {path}: itemName still has the typo");
                ok = false;
            }
        }

        if (ok)
            report.AppendLine($"[OK ] {path}");
        return ok;
    }

    #endregion
}
