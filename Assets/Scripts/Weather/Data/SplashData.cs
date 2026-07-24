using UnityEngine;

[CreateAssetMenu(menuName = "Weather/Splash Data", fileName = "New Splash Data")]
public class SplashData : ScriptableObject
{
    [Header("Visuals")]
    public Sprite[] splashSprites;
    public Material splashMaterial;

    [Header("Animation")]
    public float lifetime = 1.8f;
    public float startScale = 0.6f;
    public float endScale = 1.4f;

    [Header("Color")]
    public Color mainColor = Color.white;
}