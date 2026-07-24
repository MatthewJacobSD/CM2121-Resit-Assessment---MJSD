using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RainEffect : MonoBehaviour
{
    private ParticleSystem rainParticles;
    private ParticleSystem.EmissionModule emission;

    [SerializeField] private float maxIntensity = 800f;

    private void Awake()
    {
        rainParticles = GetComponent<ParticleSystem>();
        emission = rainParticles.emission;
    }

    public void SetActive(bool active)
    {
        if (active)
            rainParticles.Play();
        else
            rainParticles.Stop();
    }

    public void SetIntensity(float intensity)
    {
        float clamped = Mathf.Clamp(intensity, 0f, maxIntensity);
        emission.rateOverTime = clamped;
    }
}