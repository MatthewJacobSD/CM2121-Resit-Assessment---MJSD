using UnityEngine;

/// <summary>
/// Fades the water-flowing ambient clip in/out based on player distance
/// to the WaterPlane. Attach to the AudioManager or a persistent object.
/// </summary>
public class WaterAmbienceZone : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("The WaterPlane transform to measure distance from.")]
    [SerializeField] private Transform waterPlane;
    [Tooltip("The AudioManager to crossfade the water clip on.")]
    [SerializeField] private AudioManager audioManager;

    [Header("Water Ambience")]
    [Tooltip("The water-flowing ambient clip.")]
    [SerializeField] private AudioClip waterClip;

    [Header("Distance Settings")]
    [Tooltip("Distance at which water ambience starts fading in.")]
    [SerializeField] private float fadeStartDistance = 20f;
    [Tooltip("Distance at which water ambience is at full volume.")]
    [SerializeField] private float fadeEndDistance = 5f;
    [Tooltip("How quickly the water ambience fades in/out (0-1 per second).")]
    [SerializeField] private float fadeSpeed = 1f;

    #endregion

    #region Private Fields

    private Transform player;
    private float currentVolume;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        player = Camera.main?.transform;
        if (audioManager == null)
            audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        if (player == null || waterPlane == null || audioManager == null || waterClip == null)
            return;

        float distance = Vector3.Distance(player.position, waterPlane.position);

        // Determine target volume based on distance.
        float targetVolume = 0f;
        if (distance <= fadeEndDistance)
            targetVolume = 1f;
        else if (distance <= fadeStartDistance)
            targetVolume = 1f - ((distance - fadeEndDistance) / (fadeStartDistance - fadeEndDistance));

        // Smoothly interpolate current volume toward target.
        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, fadeSpeed * Time.deltaTime);

        // Apply volume to a dedicated water AudioSource (managed here, not via crossfade).
        ApplyWaterVolume(currentVolume);
    }

    #endregion

    #region Private Methods

    private AudioSource waterSource;

    private void ApplyWaterVolume(float volume)
    {
        if (waterSource == null)
        {
            // Create a dedicated AudioSource for water ambience on first use.
            waterSource = gameObject.AddComponent<AudioSource>();
            waterSource.clip = waterClip;
            waterSource.loop = true;
            waterSource.playOnAwake = false;
            waterSource.spatialBlend = 0f;
            waterSource.volume = 0f;
            waterSource.Play();
        }

        waterSource.volume = volume;

        // Stop playback when fully faded out to save CPU.
        if (volume <= 0.001f && waterSource.isPlaying)
            waterSource.Stop();
        else if (volume > 0.001f && !waterSource.isPlaying)
            waterSource.Play();
    }

    #endregion
}
