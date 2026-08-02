using UnityEngine;

/// <summary>
/// Controls the sunny weather visuals: brightens the directional light and
/// plays or stops the god-rays particle effect.
/// </summary>
[RequireComponent(typeof(Light))]
public class SunnyEffect : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("Sun light. Falls back to the component's own Light.")]
    [SerializeField] private Light sunLight;
    [SerializeField] private ParticleSystem godRays;

    #endregion

    #region Private Fields

    private float defaultIntensity = 1f;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (sunLight == null)
            sunLight = GetComponent<Light>();

        if (sunLight != null)
            defaultIntensity = sunLight.intensity;
    }

    #endregion

    #region Public Methods

    /// <summary>Turns the sunny effect on or off.</summary>
    public void SetActive(bool active)
    {
        if (sunLight != null)
            sunLight.intensity = active ? 1.8f : defaultIntensity;

        if (godRays != null)
        {
            if (active) godRays.Play();
            else godRays.Stop();
        }
    }

    #endregion
}
