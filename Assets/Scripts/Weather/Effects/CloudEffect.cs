using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CloudEffect : MonoBehaviour
{
    private ParticleSystem particles;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        mainModule = particles.main;
        emissionModule = particles.emission;
    }

    public void SetCloudColor(Color color)
    {
        mainModule.startColor = color;
    }

    public void SetEmissionRate(float rate)
    {
        emissionModule.rateOverTime = rate;
    }

    public void SetCloudy(bool active)
    {
        if (active)
            particles.Play();
        else
            particles.Stop();
    }
}