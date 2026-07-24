using UnityEngine;
[RequireComponent(typeof(AudioSource))]

[RequireComponent(typeof(CharacterController))]
public class PlayerFootstepAudio : MonoBehaviour
{
    public enum SurfaceType
    {
        DryGrass,
        WetGrass
    }

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterController characterController;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] dryFootsteps;
    [SerializeField] private AudioClip[] wetFootsteps;
    [SerializeField] private AudioClip[] runningFootsteps;

    [Header("Splash Effects")]
    [SerializeField] private SplashSpawner splashSpawner;
    [SerializeField] private SplashData wetGrassSplash;


    [Header("Settings")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;

    private SurfaceType currentSurface = SurfaceType.DryGrass;
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
        if (characterController == null)
            return;

        // Don't play footsteps while airborne.
        if (!characterController.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        // Only play footsteps when the player is moving.
        if (!playerMovement.IsMoving)
        {
            stepTimer = 0f;
            return;
        }

        // Use a shorter interval when sprinting.
        float interval = playerMovement != null && playerMovement.IsSprinting
            ? sprintStepInterval
            : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        AudioClip[] clips;

        // Select footstep sounds depending on surface and movement speed.
        if (currentSurface == SurfaceType.DryGrass)
        {
            clips = playerMovement.IsSprinting
                ? runningFootsteps
                : dryFootsteps;
        }
        else
        {
            clips = wetFootsteps;
        }


        // Play footstep audio if clips exist.
        if (clips != null && clips.Length > 0)
        {
            AudioClip clip =
                clips[Random.Range(0, clips.Length)];

            audioSource.PlayOneShot(clip);
        }


        // Spawn water splash when walking on wet grass.
        if (currentSurface == SurfaceType.WetGrass &&
            splashSpawner != null &&
            wetGrassSplash != null)
        {
            splashSpawner.SpawnSplash(
                transform.position,
                wetGrassSplash
            );
        }
    }

    /// <summary>
    /// Called by the weather system.
    /// </summary>
    public void SetSurface(SurfaceType surface)
    {
        currentSurface = surface;
    }

    public SurfaceType GetCurrentSurface()
    {
        return currentSurface;
    }
}