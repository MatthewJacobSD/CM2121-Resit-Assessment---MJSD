using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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

    private AudioSource ambientSourceA;
    private AudioSource ambientSourceB;
    private AudioSource sfxSource;
    private Coroutine crossfadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    private void SetupAudioSources()
    {
        ambientSourceA = gameObject.AddComponent<AudioSource>();
        ambientSourceA.loop = true;

        ambientSourceB = gameObject.AddComponent<AudioSource>();
        ambientSourceB.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void CrossfadeAmbient(AudioClip newClip)
    {
        if (crossfadeRoutine != null)
            StopCoroutine(crossfadeRoutine);

        crossfadeRoutine = StartCoroutine(CrossfadeCoroutine(newClip));
    }

    private IEnumerator CrossfadeCoroutine(AudioClip newClip)
    {
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

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySuccessSFX() => PlaySFX(successClip);
    public void PlayErrorSFX() => PlaySFX(errorClip);
    public void PlayPickupSFX() => PlaySFX(pickupClip);
    public void PlayDropSFX() => PlaySFX(dropClip);
    public void PlayAchievementSFX() => PlaySFX(achievementClip);

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSourceA.isPlaying) ambientSourceA.volume = ambientVolume;
        if (ambientSourceB.isPlaying) ambientSourceB.volume = ambientVolume;
    }
}