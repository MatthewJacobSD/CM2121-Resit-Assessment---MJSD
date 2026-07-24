using UnityEngine;

public class LightingEffect : MonoBehaviour
{
    [SerializeField] private LightingFlash lightningFlash;

    private void Awake()
    {
        if (lightningFlash == null)
            lightningFlash = GetComponentInChildren<LightingFlash>();
    }

    public void SetActive(bool active)
    {
        if (lightningFlash == null) return;

        if (active)
            lightningFlash.StartFlashing();
        else
            lightningFlash.StopFlashing();
    }
}