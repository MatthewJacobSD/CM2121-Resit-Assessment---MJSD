using UnityEngine;

[System.Serializable]
public class WeatherEffectParameters : MonoBehaviour
{
    [Header("Cloud Settings")]
    public Color cloudColor = new(0.5f, 0.5f, 0.5f, 0.8f);

    [Range(0, 1000)]
    public float cloudEmissionRate = 200f;

    [Range(0, 1000)]
    public float rainEmissionRate = 0f;

    [Range(0, 1000)]
    public float windSpeed = 0f;

    [Header("Special Effects")]
    public bool lightingActive = false;
    public bool sunRaysActive = false;

    public void CopyFrom(WeatherEffectParameters other)
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
