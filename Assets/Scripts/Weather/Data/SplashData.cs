using UnityEngine;

/// <summary>
/// Data asset describing how a water splash should look and animate, shared by
/// splash effects. Public fields are required for Inspector serialization.
/// </summary>
[CreateAssetMenu(menuName = "Weather/Splash Data", fileName = "New Splash Data")]
public class SplashData : ScriptableObject
{
    #region Serialized Fields

    [Header("Visuals")]
    public Sprite[] splashSprites;
    public Material splashMaterial;

    [Header("Animation")]
    public float lifetime = 1.8f;
    public float startScale = 0.6f;
    public float endScale = 1.4f;

    [Header("Color")]
    public Color mainColor = Color.white;

    #endregion
}
