using UnityEngine;

/// <summary>
/// Controls the cloud particle system: its colour, emission rate and whether
/// clouds are currently visible. Manages its own GameObject activation lifecycle.
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
        if (particles == null) return;

        mainModule.startColor = color;
    }

    /// <summary>Sets the cloud particle emission rate.</summary>
    public void SetEmissionRate(float rate)
    {
        if (particles == null) return;

        emissionModule.rateOverTime = rate;
    }

    /// <summary>Shows or hides the cloud particles. Activates/deactivates the GameObject.</summary>
    public void SetCloudy(bool active)
    {
        if (active)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (particles != null)
                particles.Play();
        }
        else
        {
            if (particles != null)
                particles.Stop();

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    #endregion
}
