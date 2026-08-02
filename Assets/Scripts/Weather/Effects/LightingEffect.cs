using UnityEngine;

/// <summary>
/// Toggles the lightning flash effect on and off.
/// </summary>
public class LightingEffect : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("Flash component that actually drives the light.")]
    [SerializeField] private LightingFlash lightningFlash;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (lightningFlash == null)
            lightningFlash = GetComponentInChildren<LightingFlash>();
    }

    #endregion

    #region Public Methods

    /// <summary>Starts or stops the lightning flashes.</summary>
    public void SetActive(bool active)
    {
        if (lightningFlash == null) return;

        if (active)
            lightningFlash.StartFlashing();
        else
            lightningFlash.StopFlashing();
    }

    #endregion
}
