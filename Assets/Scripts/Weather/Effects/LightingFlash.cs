using System.Collections;
using UnityEngine;

/// <summary>
/// Randomly flashes a light at irregular intervals while active, simulating
/// lightning in the sky.
/// </summary>
[RequireComponent(typeof(Light))]
public class LightingFlash : MonoBehaviour
{
    #region Serialized Fields

    [Header("Flash Settings")]
    [SerializeField] private float flashIntensity = 5f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float minInterval = 5f;
    [SerializeField] private float maxInterval = 15f;

    #endregion

    #region Private Fields

    private Light flashLight;
    private float defaultIntensity;
    private Coroutine flashRoutine;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        flashLight = GetComponent<Light>();
        defaultIntensity = flashLight.intensity;
        flashLight.enabled = false;
    }

    #endregion

    #region Public Methods

    /// <summary>Begins the random flash loop.</summary>
    public void StartFlashing()
    {
        if (flashLight == null) return;

        if (flashRoutine == null)
            flashRoutine = StartCoroutine(FlashRoutine());
    }

    /// <summary>Stops the flash loop and disables the light.</summary>
    public void StopFlashing()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (flashLight != null)
            flashLight.enabled = false;
    }

    #endregion

    #region Private Methods

    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            yield return StartCoroutine(SingleFlash());
        }
    }

    private IEnumerator SingleFlash()
    {
        flashLight.intensity = flashIntensity;
        flashLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashLight.intensity = defaultIntensity;
        flashLight.enabled = false;
    }

    #endregion
}
