using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private WeatherState weatherState;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Walking (dry)")]
    [SerializeField] private AudioClip[] dryWalkFootsteps;

    [Header("Running (same clips regardless of weather)")]
    [SerializeField] private AudioClip[] runningFootsteps;

    [Header("Wet Walking")]
    [SerializeField] private AudioClip[] wetWalkFootsteps;

    [Header("Splash Effects")]
    [SerializeField] private SplashSpawner splashSpawner;
    [SerializeField] private SplashData wetGrassSplash;

    [Header("Settings")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;

    private float stepTimer;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (characterController == null || !characterController.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        if (playerMovement == null || !playerMovement.IsMoving)
        {
            stepTimer = 0f;
            return;
        }

        bool sprinting = playerMovement.IsSprinting;

        float interval = sprinting ? sprintStepInterval : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep(sprinting);
            stepTimer = 0f;
        }
    }

    private void PlayFootstep(bool sprinting)
    {
        AudioClip[] clips;

        if (sprinting)
        {
            clips = runningFootsteps;
        }
        else
        {
            bool isWet = IsWetWeather();
            clips = isWet ? wetWalkFootsteps : dryWalkFootsteps;
        }

        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (!sprinting && IsWetWeather() && splashSpawner != null && wetGrassSplash != null)
        {
            splashSpawner.SpawnSplash(transform.position, wetGrassSplash);
        }
    }

    private bool IsWetWeather()
    {
        if (weatherState == null) return false;
        WeatherState.State state = weatherState.GetCurrentState();
        return state == WeatherState.State.Rainy || state == WeatherState.State.Stormy;
    }
}
