using UnityEngine;
using System.Collections;

// Ensures this GameObject always has a Light component.
// The lightning flash effect requires a light source to create flashes.
[RequireComponent(typeof(Light))]
public class LightingFlash : MonoBehaviour
{
    // The light component used to create the lightning flash.
    private Light flashLight;


    // Stores the original light intensity so it can be restored
    // after the lightning flash finishes.
    private float defaultIntensity;


    // The brightness of the lightning flash.
    // Higher values create stronger lightning strikes.
    [SerializeField] private float flashIntensity = 5f;


    // How long the lightning flash remains visible.
    [SerializeField] private float flashDuration = 0.1f;


    // Minimum time between automatic lightning strikes.
    [SerializeField] private float minInterval = 5f;


    // Maximum time between automatic lightning strikes.
    [SerializeField] private float maxInterval = 15f;


    // Stores the currently running lightning coroutine.
    // Used to prevent multiple lightning loops running at once.
    private Coroutine flashRoutine;


    private void Awake()
    {
        // Get the Light component attached to this GameObject.
        flashLight = GetComponent<Light>();

        // Store the starting intensity so it can be restored later.
        defaultIntensity = flashLight.intensity;

        // Disable the light at the beginning.
        // Lightning should only appear when triggered.
        flashLight.enabled = false;
    }


    // Starts the automatic lightning cycle.
    // Lightning will randomly flash between the defined intervals.
    public void StartFlashing()
    {
        // Prevent multiple flashing coroutines from being created.
        if (flashRoutine == null)
            flashRoutine = StartCoroutine(FlashRoutine());
    }


    // Stops the automatic lightning cycle.
    public void StopFlashing()
    {
        // Stop the active coroutine if one exists.
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }


        // Ensure the lightning light is disabled.
        flashLight.enabled = false;
    }


    // Creates one immediate lightning strike.
    // Can be called manually from gameplay events.
    public void TriggerFlash()
    {
        StartCoroutine(SingleFlash());
    }


    // Repeatedly creates lightning strikes at random intervals.
    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            // Wait for a random amount of time before the next strike.
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));


            // Perform one lightning flash.
            yield return StartCoroutine(SingleFlash());
        }
    }


    // Handles the actual lightning flash behaviour.
    private IEnumerator SingleFlash()
    {
        // Increase brightness and enable the lightning light.
        flashLight.intensity = flashIntensity;
        flashLight.enabled = true;


        // Keep the flash visible for the configured duration.
        yield return new WaitForSeconds(flashDuration);


        // Restore the original lighting settings.
        flashLight.intensity = defaultIntensity;
        flashLight.enabled = false;
    }
}