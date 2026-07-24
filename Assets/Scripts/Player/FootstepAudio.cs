using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class PlayerFootstepAudio : MonoBehaviour
{
    public enum SurfaceType { DryGrass, WetGrass }

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] dryFootsteps;
    [SerializeField] private AudioClip[] wetFootsteps;
    [SerializeField] private AudioClip[] runningFootsteps;

    [Header("Settings")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;

    private SurfaceType currentSurface = SurfaceType.DryGrass;
    private float stepTimer;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (!playerMovement || !playerMovement.IsMoving || !GetComponent<CharacterController>().isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        float interval = playerMovement.IsSprinting ? sprintStepInterval : walkStepInterval;
        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        AudioClip[] clips = (currentSurface == SurfaceType.DryGrass)
            ? (playerMovement.IsSprinting ? runningFootsteps : dryFootsteps)
            : wetFootsteps;

        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    public void SetSurface(SurfaceType surface)
    {
        currentSurface = surface;
    }
}