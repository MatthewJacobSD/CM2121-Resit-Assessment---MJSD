using UnityEngine;

[System.Serializable]
public class WeatherEffectParameters
{
    [Header("Cloud Settings")]
    public Color cloudColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    [Range(0, 1000)]
    public float cloudEmissionRate = 200f;

    [Range(0, 1000)]
    public float rainEmissionRate = 0f;

    [Range(0, 1000)]
    public float windSpeed = 0f;

    [Header("Special Effects")]
    public bool lightingActive = false;
    public bool sunRaysActive = false;

    // Optional: Constructor for easy default values
    public WeatherEffectParameters() { }

    public WeatherEffectParameters(WeatherEffectParameters other)
    {
        if (other == null) return;

        cloudColor = other.cloudColor;
        cloudEmissionRate = other.cloudEmissionRate;
        rainEmissionRate = other.rainEmissionRate;
        windSpeed = other.windSpeed;
        lightingActive = other.lightingActive;
        sunRaysActive = other.sunRaysActive;
    }
}