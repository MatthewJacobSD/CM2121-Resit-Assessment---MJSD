using UnityEngine;

/// <summary>
/// Plays a one-shot splash particle burst at a position using splash data.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SplashEffect : MonoBehaviour
{
    #region Private Fields

    private ParticleSystem splashParticles;
    private ParticleSystemRenderer particleRenderer;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        splashParticles = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        splashParticles.Stop();
    }

    #endregion

    #region Public Methods

    /// <summary>Plays a splash burst at the given position with the given data.</summary>
    public void PlaySplash(Vector3 position, SplashData data)
    {
        if (data == null) return;

        transform.position = position;

        if (particleRenderer != null && data.splashMaterial != null)
            particleRenderer.material = data.splashMaterial;

        ParticleSystem.MainModule main = splashParticles.main;
        main.startLifetime = data.lifetime;

        splashParticles.Play();
    }

    #endregion
}
