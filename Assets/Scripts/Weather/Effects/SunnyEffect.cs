using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunnyEffect : MonoBehaviour
{
    [SerializeField] private Light sunLight;
    [SerializeField] private ParticleSystem godRays;

    private float defaultIntensity = 1f;

    private void Awake()
    {
        if (sunLight == null)
            sunLight = GetComponent<Light>();

        if (sunLight != null)
            defaultIntensity = sunLight.intensity;
    }

    public void SetActive(bool active)
    {
        if (sunLight != null)
            sunLight.intensity = active ? 1.8f : defaultIntensity;

        if (godRays != null)
        {
            if (active) godRays.Play();
            else godRays.Stop();
        }
    }
}