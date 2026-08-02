using UnityEngine;

/// <summary>
/// Controls the cloud particle system: its colour, emission rate and whether
/// clouds are currently visible.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class CloudEffect : MonoBehaviour
{
    #region Private Fields

    private ParticleSystem particles;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        mainModule = particles.main;
        emissionModule = particles.emission;
    }

    #endregion

    #region Public Methods

    /// <summary>Sets the cloud particle colour.</summary>
    public void SetCloudColor(Color color)
    {
        mainModule.startColor = color;
    }

    /// <summary>Sets the cloud particle emission rate.</summary>
    public void SetEmissionRate(float rate)
    {
        emissionModule.rateOverTime = rate;
    }

    /// <summary>Shows or hides the cloud particles.</summary>
    public void SetCloudy(bool active)
    {
        if (active)
            particles.Play();
        else
            particles.Stop();
    }

    #endregion
}
