using UnityEngine;

/// <summary>
/// Controls the sunny weather visuals: brightens the directional light and
/// plays or stops the god-rays particle effect. Manages its own GameObject
/// activation lifecycle — active only during Sunny weather.
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

    /// <summary>Turns the sunny effect on or off. Activates/deactivates the GameObject.</summary>
    public void SetActive(bool active)
    {
        if (active)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (sunLight != null)
                sunLight.intensity = 1.8f;

            if (godRays != null)
                godRays.Play();
        }
        else
        {
            if (sunLight != null)
                sunLight.intensity = defaultIntensity;

            if (godRays != null)
                godRays.Stop();

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    #endregion
}
