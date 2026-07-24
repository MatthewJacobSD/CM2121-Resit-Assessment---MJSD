using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightingFlash : MonoBehaviour
{
    private Light flashLight;
    private float defaultIntensity;

    [SerializeField] private float flashIntensity = 5f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float minInterval = 5f;
    [SerializeField] private float maxInterval = 15f;

    private Coroutine flashRoutine;

    private void Awake()
    {
        flashLight = GetComponent<Light>();
        defaultIntensity = flashLight.intensity;
        flashLight.enabled = false;
    }

    public void StartFlashing()
    {
        if (flashRoutine == null)
            flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlashing()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        flashLight.enabled = false;
    }

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
}