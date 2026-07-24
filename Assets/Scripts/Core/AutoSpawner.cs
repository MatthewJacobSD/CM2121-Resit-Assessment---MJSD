using UnityEngine;

public class AutoSpawner : MonoBehaviour
{
    [Header("Prefabs to auto-create if missing")]
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject hudManagerPrefab;
    [SerializeField] private GameObject scoreManagerPrefab;

    private void Awake()
    {
        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            var go = Instantiate(audioManagerPrefab);
            go.name = "AudioManager_Auto";
        }

        if (ScoreManager.Instance == null && scoreManagerPrefab != null)
        {
            var go = Instantiate(scoreManagerPrefab);
            go.name = "ScoreManager_Auto";
        }

        if (FindAnyObjectByType<HUDManager>() == null && hudManagerPrefab != null)
        {
            var go = Instantiate(hudManagerPrefab);
            go.name = "HUDManager_Auto";
        }
    }
}
