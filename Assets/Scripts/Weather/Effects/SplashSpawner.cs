using UnityEngine;

/// <summary>
/// Instantiates splash effects at runtime from a prefab, driven by footstep audio.
/// </summary>
public class SplashSpawner : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private SplashEffect splashPrefab;

    #endregion

    #region Public Methods

    /// <summary>Instantiates and plays a splash at the given world position.</summary>
    public void SpawnSplash(Vector3 position, SplashData data)
    {
        if (splashPrefab == null || data == null) return;

        SplashEffect splash = Instantiate(splashPrefab, position, Quaternion.identity);
        splash.PlaySplash(position, data);
        Destroy(splash.gameObject, data.lifetime + 0.5f);
    }

    #endregion
}
