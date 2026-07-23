using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunnyEffect : MonoBehaviour
{
    // Reference to the main directional light used as the sun.
    // This controls the overall sunlight intensity in the scene.
    [SerializeField] private Light directionalSunLight;

    // Optional particle system used to create a "god rays" / sun beam effect.
    // This is usually a transparent particle effect placed around the sun direction.
    [SerializeField] private ParticleSystem godRaysParticles;


    // Stores the original sunlight intensity so it can be restored
    // when the sunny weather effect is disabled.
    private float defaultIntensity = 1f;


    private void Awake()
    {
        // If no directional light has been assigned in the Inspector,
        // try to find one automatically.
        if (directionalSunLight == null)
            directionalSunLight = GetComponent<Light>() ?? FindFirstObjectByType<Light>();


        // Store the starting intensity of the sunlight.
        // This allows the effect to return to the original lighting value later.
        if (directionalSunLight != null)
            defaultIntensity = directionalSunLight.intensity;
    }


    // Activates the sunny weather effect.
    // Increases sunlight intensity and enables god rays if available.
    public void ActiveSunnyEffect()
    {
        // Increase brightness to create a stronger sunny appearance.
        if (directionalSunLight != null)
            directionalSunLight.intensity = 1.8f;


        // Start the god rays particle effect if it is not already playing.
        if (godRaysParticles != null && !godRaysParticles.isPlaying)
            godRaysParticles.Play();
    }


    // Deactivates the sunny weather effect.
    // Restores the original sunlight intensity and disables god rays.
    public void DeactivateSunnyEffect()
    {
        // Restore the original light intensity.
        if (directionalSunLight != null)
            directionalSunLight.intensity = defaultIntensity;


        // Stop the god rays particle effect.
        if (godRaysParticles != null)
            godRaysParticles.Stop();
    }
}