using UnityEngine;

/// <summary>
/// Configurable per-weather-state values used when transitioning visual effects.
/// Public fields are required so they are serialized in the Inspector.
/// </summary>
[System.Serializable]
public class WeatherEffectParameters : MonoBehaviour
{
    #region Serialized Fields

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

    #endregion

    #region Public Methods

    /// <summary>Copies all values from another parameters asset.</summary>
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

    #endregion
}
