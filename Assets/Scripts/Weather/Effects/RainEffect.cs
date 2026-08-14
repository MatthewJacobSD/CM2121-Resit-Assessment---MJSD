using UnityEngine;

/// <summary>
/// Controls the rain particle system: toggles it on/off and adjusts intensity.
/// Manages its own GameObject activation lifecycle — inactive when not needed,
/// activated on demand when the weather state requires rain.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class RainEffect : MonoBehaviour
{
    #region Serialized Fields

    [Header("Settings")]
    [Tooltip("Upper clamp for the emission rate.")]
    [SerializeField] private float maxIntensity = 800f;

    #endregion

    #region Private Fields

    private ParticleSystem rainParticles;
    private ParticleSystem.EmissionModule emission;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rainParticles = GetComponent<ParticleSystem>();
        emission = rainParticles.emission;
    }

    #endregion

    #region Public Methods

    /// <summary>Turns the rain on or off. Activates/deactivates the GameObject as needed.</summary>
    public void SetActive(bool active)
    {
        if (active)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (rainParticles != null)
                rainParticles.Play();
        }
        else
        {
            if (rainParticles != null)
                rainParticles.Stop();

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    /// <summary>Sets the rain emission rate, clamped to the configured maximum.</summary>
    public void SetIntensity(float intensity)
    {
        if (rainParticles == null) return;

        float clamped = Mathf.Clamp(intensity, 0f, maxIntensity);
        emission.rateOverTime = clamped;
    }

    #endregion
}
