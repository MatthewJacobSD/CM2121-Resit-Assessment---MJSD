using System.Collections;
using UnityEngine;

/// <summary>
/// Central audio manager: crossfades ambient clips between weather states and
/// plays one-shot sound effects. Exposes convenience wrappers for SFX.
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Ambient Audio")]
    [SerializeField] private AudioClip sunnyClip;
    [SerializeField] private AudioClip cloudyClip;
    [SerializeField] private AudioClip windyClip;
    [SerializeField] private AudioClip rainyClip;
    [SerializeField] private AudioClip stormyClip;

    [Header("Ambient Settings")]
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.5f;
    [SerializeField] private float crossfadeDuration = 2f;

    [Header("SFX")]
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip achievementClip;

    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.7f;

    #endregion

    #region Private Fields

    // Two sources are used so one can fade out while the other fades in.
    private AudioSource ambientSourceA;
    private AudioSource ambientSourceB;
    private AudioSource sfxSource;
    private Coroutine crossfadeRoutine;

    #endregion

    #region Public Properties

    public static AudioManager Instance { get; private set; }

    #endregion

    #region Unity Lifecycle

    // Singleton pattern: keep one persistent instance across scene reloads.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (transform.root == transform)
            DontDestroyOnLoad(gameObject);
        SetupAudioSources();
    }

    private void Start()
    {
        if (sunnyClip != null)
        {
            ambientSourceA.clip = sunnyClip;
            ambientSourceA.volume = ambientVolume;
            ambientSourceA.Play();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>Crossfades the ambient loop from the current clip to the given one.</summary>
    public void CrossfadeAmbient(AudioClip newClip)
    {
        if (crossfadeRoutine != null)
            StopCoroutine(crossfadeRoutine);

        crossfadeRoutine = StartCoroutine(CrossfadeCoroutine(newClip));
    }

    /// <summary>Plays a one-shot sound effect at the configured SFX volume.</summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>Plays the success sound effect.</summary>
    public void PlaySuccessSFX() => PlaySFX(successClip);

    /// <summary>Plays the error sound effect.</summary>
    public void PlayErrorSFX() => PlaySFX(errorClip);

    /// <summary>Plays the item pickup sound effect.</summary>
    public void PlayPickupSFX() => PlaySFX(pickupClip);

    /// <summary>Plays the item drop sound effect.</summary>
    public void PlayDropSFX() => PlaySFX(dropClip);

    /// <summary>Plays the achievement sound effect.</summary>
    public void PlayAchievementSFX() => PlaySFX(achievementClip);

    /// <summary>Sets the ambient volume, clamped to [0, 1].</summary>
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSourceA.isPlaying) ambientSourceA.volume = ambientVolume;
        if (ambientSourceB.isPlaying) ambientSourceB.volume = ambientVolume;
    }

    #endregion

    #region Private Methods

    private void SetupAudioSources()
    {
        ambientSourceA = gameObject.AddComponent<AudioSource>();
        ambientSourceA.loop = true;

        ambientSourceB = gameObject.AddComponent<AudioSource>();
        ambientSourceB.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
        // Fade out the source that is currently playing, fade in the other.
        AudioSource fadeOut = ambientSourceA.isPlaying ? ambientSourceA : ambientSourceB;
        AudioSource fadeIn = fadeOut == ambientSourceA ? ambientSourceB : ambientSourceA;

        fadeIn.clip = newClip;
        fadeIn.volume = 0f;
        fadeIn.Play();

        float elapsed = 0f;
        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeDuration;
            fadeOut.volume = Mathf.Lerp(ambientVolume, 0f, t);
            fadeIn.volume = Mathf.Lerp(0f, ambientVolume, t);
            yield return null;
        }

        fadeOut.Stop();
        fadeOut.volume = 0f;
        fadeIn.volume = ambientVolume;
    }

    #endregion
}
