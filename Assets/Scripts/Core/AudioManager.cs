using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Ambient Clips")]
    [SerializeField] private AudioClip sunnyClip;
    [SerializeField] private AudioClip cloudyClip;
    [SerializeField] private AudioClip windyClip;
    [SerializeField] private AudioClip rainyClip;
    [SerializeField] private AudioClip stormyClip;

    [Header("Ambient Settings")]
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.5f;
    [SerializeField] private float crossfadeDuration = 2f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip achievementClip;

    [Header("SFX Settings")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.7f;

    private AudioSource ambientSourceA;
    private AudioSource ambientSourceB;
    private AudioSource sfxSource;
    private Coroutine crossfadeRoutine;
    private WeatherState subscribedWeatherState;


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


    private void OnEnable()
    {
        SubscribeToWeather();
    }


    private void OnDisable()
    {
        UnsubscribeFromWeather();
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
        ambientSourceA.playOnAwake = false;

        ambientSourceB = gameObject.AddComponent<AudioSource>();
        ambientSourceB.loop = true;
        ambientSourceB.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }


    private void SubscribeToWeather()
    {
        var weatherState = FindFirstObjectByType<WeatherState>();
        if (weatherState != null)
        {
            subscribedWeatherState = weatherState;
            weatherState.OnWeatherChanged += HandleWeatherChanged;
        }
    }


    private void UnsubscribeFromWeather()
    {
        if (subscribedWeatherState != null)
        {
            subscribedWeatherState.OnWeatherChanged -= HandleWeatherChanged;
            subscribedWeatherState = null;
        }
    }


    private void HandleWeatherChanged(WeatherState.State state)
    {
        AudioClip targetClip = GetClipForWeather(state);

        if (targetClip != null)
            CrossfadeAmbient(targetClip);
    }


    private AudioClip GetClipForWeather(WeatherState.State state)
    {
        switch (state)
        {
            case WeatherState.State.Sunny: return sunnyClip;
            case WeatherState.State.Cloudy: return cloudyClip;
            case WeatherState.State.Windy: return windyClip;
            case WeatherState.State.Rainy: return rainyClip;
            case WeatherState.State.Stormy: return stormyClip;
            default: return sunnyClip;
        }
    }


    private void CrossfadeAmbient(AudioClip newClip)
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
            float t = Mathf.Clamp01(elapsed / crossfadeDuration);
            fadeOut.volume = Mathf.Lerp(ambientVolume, 0f, t);
            fadeIn.volume = Mathf.Lerp(0f, ambientVolume, t);
            yield return null;
        }

        fadeOut.Stop();
        fadeOut.volume = 0f;
        fadeIn.volume = ambientVolume;
        crossfadeRoutine = null;
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }


    public void PlaySuccessSFX()
    {
        PlaySFX(successClip);
    }


    public void PlayErrorSFX()
    {
        PlaySFX(errorClip);
    }


    public void PlayPickupSFX()
    {
        PlaySFX(pickupClip);
    }


    public void PlayDropSFX()
    {
        PlaySFX(dropClip);
    }


    public void PlayAchievementSFX()
    {
        PlaySFX(achievementClip);
    }


    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);

        if (ambientSourceA.isPlaying)
            ambientSourceA.volume = ambientVolume;
        if (ambientSourceB.isPlaying)
            ambientSourceB.volume = ambientVolume;
    }


    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}