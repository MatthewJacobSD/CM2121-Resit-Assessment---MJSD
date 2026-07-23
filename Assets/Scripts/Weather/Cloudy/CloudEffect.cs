using System;
using UnityEngine;

// Ensures that this GameObject always has a ParticleSystem component attached.
// The script will not work without one.
[RequireComponent(typeof(ParticleSystem))]
public class CloudEffect : MonoBehaviour
{
    // Reference to the particle system used to display clouds.
    private ParticleSystem cloudParticles;

    // Allows access to and modification of the particle system's main settings,
    // such as particle colour.
    private ParticleSystem.MainModule mainModule;

    // Allows access to and modification of emission settings,
    // such as the number of cloud particles generated over time.
    private ParticleSystem.EmissionModule emissionModule;


    // Default colour applied to clouds when the effect starts.
    // Alpha controls cloud transparency.
    [SerializeField] private Color defaultCloudColor = new(0.5f, 0.5f, 0.5f, 0.8f);

    // Default amount of cloud particles emitted per second.
    [SerializeField] private float defaultEmissionRate = 200f;

    [SerializeField] private ParticleSystem sunnyClouds;
    [SerializeField] private ParticleSystem defaultClouds;
    [SerializeField] private ParticleSystem stormClouds;


    private void Awake()
    {
        // Gets the ParticleSystem component attached to this GameObject.
        cloudParticles = GetComponent<ParticleSystem>();

        // Stores references to the ParticleSystem modules.
        // These allow us to change particle settings during gameplay.
        mainModule = cloudParticles.main;
        emissionModule = cloudParticles.emission;

        // Apply the default cloud appearance when the game starts.
        SetCloudDarkness(defaultCloudColor);
        SetCloudEmissionRate(defaultEmissionRate);
    }


    // Changes the colour and transparency of the cloud particles.
    // Used by the WeatherEffects manager when transitioning between weather states.
    public void SetCloudDarkness(Color cloudColor)
    {
        mainModule.startColor = cloudColor;
    }


    // Changes the number of cloud particles being generated.
    // Higher values create denser clouds, lower values create thinner clouds.
    public void SetCloudEmissionRate(float emissionRate)
    {
        emissionModule.rateOverTime = emissionRate;
    }


    // Starts playing the cloud particle effect.
    // Called when clouds need to appear.
    public void Activate()
    {
        cloudParticles.Play();
    }


    // Stops playing the cloud particle effect.
    // Called when clouds need to disappear.
    public void Deactivate()
    {
        cloudParticles.Stop();
    }

    public void SetSunny()
    {
        sunnyClouds.Play();

        defaultClouds.Stop();

        stormClouds.Stop();
    }

    public void SetCloudy()
    {
        sunnyClouds.Stop();

        defaultClouds.Play();

        stormClouds.Stop();
    }

    public void SetStormy()
    {
        sunnyClouds.Stop();

        defaultClouds.Stop();

        stormClouds.Play();
    }
}