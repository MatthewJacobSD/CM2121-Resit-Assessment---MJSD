using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SplashEffect : MonoBehaviour
{
    private ParticleSystem splashParticles;
    private ParticleSystemRenderer particleRenderer;

    private void Awake()
    {
        splashParticles = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        splashParticles.Stop();
    }

    public void PlaySplash(Vector3 position, SplashData data)
    {
        if (data == null) return;

        transform.position = position;

        if (particleRenderer != null && data.splashMaterial != null)
            particleRenderer.material = data.splashMaterial;

        var main = splashParticles.main;
        main.startLifetime = data.lifetime;

        splashParticles.Play();
    }
}