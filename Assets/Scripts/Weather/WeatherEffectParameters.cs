using System;
using UnityEngine;

[Serializable]
public class WeatherEffectParameters : MonoBehaviour
{
    public Color cloudColor;
    [Range(0, 1000)] public float cloudEmissionRate;
    [Range(0, 1000)] public float rainEmissionRate;
    [Range(0, 1000)] public float windSpeed;
    public bool lightingActive;
    public bool sunRaysActive;
}