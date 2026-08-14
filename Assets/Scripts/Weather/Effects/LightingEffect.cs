using UnityEngine;

/// <summary>
/// Toggles the lightning flash effect on and off.
/// Manages its own GameObject activation lifecycle — active only during Stormy weather.
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

    /// <summary>Starts or stops the lightning flashes. Activates/deactivates the GameObject.</summary>
    public void SetActive(bool active)
    {
        if (active)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (lightningFlash != null)
                lightningFlash.StartFlashing();
        }
        else
        {
            if (lightningFlash != null)
                lightningFlash.StopFlashing();

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    #endregion
}
