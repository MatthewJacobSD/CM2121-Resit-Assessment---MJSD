using UnityEngine;

public class SplashSpawner : MonoBehaviour
{
    [SerializeField] private SplashEffect splashPrefab;

    public void SpawnSplash(Vector3 position, SplashData data)
    {
        if (splashPrefab == null || data == null) return;

        SplashEffect splash = Instantiate(splashPrefab, position, Quaternion.identity);
        splash.PlaySplash(position, data);
    }
}