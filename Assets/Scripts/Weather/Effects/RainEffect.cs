using UnityEngine;

/// <summary>
/// Controls the rain particle system: toggles it on/off and adjusts intensity.
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

    /// <summary>Turns the rain on or off.</summary>
    public void SetActive(bool active)
    {
        if (active)
            rainParticles.Play();
        else
            rainParticles.Stop();
    }

    /// <summary>Sets the rain emission rate, clamped to the configured maximum.</summary>
    public void SetIntensity(float intensity)
    {
        float clamped = Mathf.Clamp(intensity, 0f, maxIntensity);
        emission.rateOverTime = clamped;
    }

    #endregion
}
