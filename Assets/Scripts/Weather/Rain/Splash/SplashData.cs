using UnityEngine;

[CreateAssetMenu(menuName = "Splash/Splash Data", fileName = "New Splash Data")]
public class SplashData : ScriptableObject
{
    [Header("Visuals")]
    public Sprite[] splashSprites;
    public Material splashMaterial;

    [Header("Rim Lighting")]
    public Color rimColor = Color.cyan;
    public float rimPower = 3f;
    public float rimIntensity = 2f;

    [Header("Animation")]
    public float lifetime = 1.8f;
    public float startScale = 0.6f;
    public float endScale = 1.4f;
    public AnimationCurve scaleCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Movement")]
    public float spawnForce = 2f;

    [Header("Colour")]
    public Color mainColor = Color.white;
}