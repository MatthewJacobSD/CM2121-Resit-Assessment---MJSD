using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SplashEffect : MonoBehaviour
{
    [Header("Current Splash Type")]
    [SerializeField] private SplashData currentData;

    private ParticleSystem splashParticles;
    private ParticleSystemRenderer particleRenderer;


    private void Awake()
    {
        splashParticles = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        splashParticles.Stop();
    }


    /// <summary>
    /// Plays a splash effect using the selected SplashData.
    /// </summary>
    public void PlaySplash(Vector3 position, SplashData data)
    {
        if (data == null)
            return;

        currentData = data;


        // Move splash effect to impact location.
        transform.position = position;


        ApplySplashSettings();


        splashParticles.Play();
    }


    private void ApplySplashSettings()
    {
        if (currentData == null)
            return;


        // Apply material variation.
        if (particleRenderer != null)
        {
            particleRenderer.material = currentData.splashMaterial;
        }


        // Apply random sprite if sprites exist.
        if (currentData.splashSprites != null &&
            currentData.splashSprites.Length > 0)
        {
            var textureAnimation =
                splashParticles.textureSheetAnimation;

            textureAnimation.enabled = true;

            textureAnimation.SetSprite(
                0,
                currentData.splashSprites[
                    Random.Range(
                        0,
                        currentData.splashSprites.Length
                    )
                ]
            );
        }


        // Apply lifetime.
        var main = splashParticles.main;
        main.startLifetime = currentData.lifetime;
    }


    public void StopSplash()
    {
        if (splashParticles.isPlaying)
            splashParticles.Stop();
    }
}