using UnityEngine;
using System.Collections;

public class LightingEffect : MonoBehaviour
{
    // Reference to the lightning flash controller.
    // This handles the visual lightning effect such as screen flashes
    // or sudden changes in lighting.
    [SerializeField] private LightingFlash lightningFlash;


    // Optional audio source for thunder sounds.
    // Currently acts as a placeholder for future thunder audio implementation.
    [SerializeField] private AudioClip thunderClip;

    private void Awake()
    {
        // If a LightningFlash component has not been assigned manually,
        // attempt to find one among this object's child objects.
        if (lightningFlash == null)
            lightningFlash = GetComponentInChildren<LightingFlash>();
    }


    // Activates the lightning effect.
    // Used when switching to stormy weather.
    public void ActiveLightingEffect()
    {
        // Begin the repeating lightning flash effect.
        if (lightningFlash)
            lightningFlash.StartFlashing();
    }


    // Deactivates the lightning effect.
    // Stops all lightning flashes when leaving stormy weather.
    public void DeactivateLightingEffect()
    {
        // Stop the repeating lightning flash effect.
        if (lightningFlash)
            lightningFlash.StopFlashing();
    }


    // Creates a single lightning strike manually.
    // Can be triggered by gameplay events or storm intervals.
    public void TriggerSingleFlash()
    {
        // Trigger one lightning flash.
        if (lightningFlash)
            lightningFlash.TriggerFlash();


        // Play thunder audio at the same time.
        // Placeholder until final sound effects are added.
        if (thunderClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(thunderClip);
    }
}