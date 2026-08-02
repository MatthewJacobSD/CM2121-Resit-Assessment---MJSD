using UnityEngine;

/// <summary>
/// Creates core manager objects at runtime if they are missing from the scene,
/// so the game remains playable even when a scene was set up without managers.
/// </summary>
public class AutoSpawner : MonoBehaviour
{
    #region Serialized Fields

    [Header("Prefabs to auto-create if missing")]
    [Tooltip("Spawned when no AudioManager exists in the scene.")]
    [SerializeField] private GameObject audioManagerPrefab;

    [Tooltip("Spawned when no HUDManager exists in the scene.")]
    [SerializeField] private GameObject hudManagerPrefab;

    [Tooltip("Spawned when no ScoreManager exists in the scene.")]
    [SerializeField] private GameObject scoreManagerPrefab;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            GameObject go = Instantiate(audioManagerPrefab);
            go.name = "AudioManager_Auto";
        }

        if (ScoreManager.Instance == null && scoreManagerPrefab != null)
        {
            GameObject go = Instantiate(scoreManagerPrefab);
            go.name = "ScoreManager_Auto";
        }

        if (FindAnyObjectByType<HUDManager>() == null && hudManagerPrefab != null)
        {
            GameObject go = Instantiate(hudManagerPrefab);
            go.name = "HUDManager_Auto";
        }
    }

    #endregion
}
