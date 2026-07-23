using UnityEngine;

// Ensures this GameObject always has a ParticleSystem component.
// The rain effect requires particles to function.
[RequireComponent(typeof(ParticleSystem))]
public class RainEffect : MonoBehaviour
{
    // Reference to the particle system responsible for creating rain.
    private ParticleSystem rainParticles;

    // Allows modification of the particle emission settings,
    // such as the amount of rain falling per second.
    private ParticleSystem.EmissionModule emissionModule;

    // Maximum number of rain particles emitted per second.
    // Prevents extremely heavy rain from affecting performance.
    [SerializeField] private float maxIntensity = 800f;


    private void Awake()
    {
        // Get the ParticleSystem attached to this GameObject.
        rainParticles = GetComponent<ParticleSystem>();

        // Store a reference to the emission module so the rain intensity
        // can be changed dynamically.
        emissionModule = rainParticles.emission;
    }


    // Controls the intensity of the rain effect.
    // Higher intensity values create heavier rainfall.
    //
    // PlayerMovement is currently unused but can be used later
    // for gameplay effects, such as changing player movement speed
    // during storms or adding environmental interactions.
    public void SetRainIntensity(float intensity, PlayerMovement player = null)
    {
        // Restrict the rain value between 0 and the maximum allowed intensity.
        float clamped = Mathf.Clamp(intensity, 0f, maxIntensity);

        // Update the particle emission rate based on weather conditions.
        emissionModule.rateOverTime = clamped;
    }


    // Starts the rain particle effect.
    // Used when switching to rainy or stormy weather.
    public void Activate()
    {
        rainParticles.Play();
    }


    // Stops the rain particle effect and rain audio.
    // Used when switching away from rainy weather.
    public void Deactivate()
    {
        rainParticles.Stop();
    }
}