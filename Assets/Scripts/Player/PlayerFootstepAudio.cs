using UnityEngine;

/// <summary>
/// Plays surface-aware footstep audio: dry/rainy clips by weather state, and
/// splash effects when the player walks through water or recently wet ground.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class PlayerFootstepAudio : MonoBehaviour
{
    #region Constants

    private const float SurfaceRayHeightOffset = 0.1f;
    private const float SurfaceRayExtraDistance = 0.5f;

    #endregion

    #region Serialized Fields

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

    [Header("Surface Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("Wetness Drying")]
    [Tooltip("Seconds after leaving rain/water before the ground is considered dry.")]
    [SerializeField] private float wetnessDuration = 5f;

    [Header("Settings")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;

    #endregion

    #region Private Fields

    private float stepTimer;
    private float wetnessTimer;
    private bool onWater;

    #endregion

    #region Unity Lifecycle

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

    #endregion

    #region Footstep Logic

    private void HandleFootsteps()
    {
        // Reset the timer while airborne or stationary so the next step lands cleanly.
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

        DetectSurface();

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
        else if (onWater)
        {
            clips = wetWalkFootsteps;
        }
        else
        {
            bool isWet = IsSurfaceWet();
            clips = isWet ? wetWalkFootsteps : dryWalkFootsteps;
        }

        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (!sprinting && (onWater || IsSurfaceWet()) && splashSpawner != null && wetGrassSplash != null)
        {
            splashSpawner.SpawnSplash(transform.position, wetGrassSplash);
        }
    }

    #endregion

    #region Surface Detection

    private void DetectSurface()
    {
        Vector3 origin = transform.position + Vector3.up * SurfaceRayHeightOffset;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance + SurfaceRayExtraDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            int layer = hit.collider.gameObject.layer;
            bool wasOnWater = onWater;
            onWater = layer == LayerMask.NameToLayer("Water");

            if (onWater)
            {
                wetnessTimer = 0f;
            }
            else if (wasOnWater && !onWater)
            {
                // Walking off water restarts the wetness countdown for the drying timer.
                wetnessTimer = 0f;
            }
        }
        else
        {
            onWater = false;
        }

        if (!onWater && IsGroundDry())
        {
            wetnessTimer += Time.deltaTime;
        }
    }

    private bool IsGroundDry()
    {
        if (weatherState == null) return true;
        WeatherState.State state = weatherState.GetCurrentState();
        return state == WeatherState.State.Sunny;
    }

    private bool IsSurfaceWet()
    {
        if (onWater) return true;

        if (weatherState != null)
        {
            WeatherState.State state = weatherState.GetCurrentState();
            if (state == WeatherState.State.Rainy || state == WeatherState.State.Stormy)
                return true;
        }

        if (wetnessTimer < wetnessDuration)
            return true;

        return false;
    }

    #endregion
}
