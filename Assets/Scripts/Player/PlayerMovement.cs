using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves the player with the CharacterController using WASD input, handles
/// jumping, gravity and an externally applied speed modifier (used by weather).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Constants

    private const float SprintInputThreshold = 0.5f;
    private const float MoveInputThreshold = 0.01f;
    private const float GroundedSnapVelocity = -2f;

    #endregion

    #region Serialized Fields

    [Header("Input")]
    [Tooltip("Input Action Asset containing the Player action map.")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8.5f;
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 25f;
    [SerializeField, Range(0f, 1f)] private float airControl = 0.6f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -18f;
    [Tooltip("Extra gravity applied while falling, for a snappier landing.")]
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    #endregion

    #region Private Fields

    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector2 moveInput;
    private Vector3 velocity;
    private Vector3 currentMovement;

    private bool isGrounded;
    private bool sprinting;

    private float speedModifier = 1f;

    // Horizontal velocity added by the weather system (storm wind push).
    private Vector3 windPush;

    #endregion

    #region Public Properties

    public bool IsSprinting => sprinting;
    public bool IsMoving => moveInput.sqrMagnitude > MoveInputThreshold;
    public float SpeedModifier => speedModifier;
    public float CurrentWalkSpeed => walkSpeed * speedModifier;
    public float CurrentSprintSpeed => sprintSpeed * speedModifier;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        InputActionMap playerMap = playerControls.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Movement", true);
        jumpAction = playerMap.FindAction("Jump", true);
        sprintAction = playerMap.FindAction("Sprint", true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    private void Update()
    {
        CheckGround();
        ReadInput();
        HandleJump();
        ApplyGravity();
        MovePlayer();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets a multiplier applied to walk/sprint speed (used by weather states).
    /// Clamped to [0.1, 2] to prevent degenerate movement.
    /// </summary>
    public void SetSpeedModifier(float modifier)
    {
        speedModifier = Mathf.Clamp(modifier, 0.1f, 2f);
    }

    /// <summary>
    /// Sets a horizontal wind velocity (m/s) added to player movement, used to
    /// gradually push the player during storms. Ramps are handled by the caller.
    /// </summary>
    public void SetWindPush(Vector3 push)
    {
        windPush = push;
    }

    #endregion

    #region Private Methods

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
        sprinting = sprintAction.ReadValue<float>() > SprintInputThreshold;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

        // Snap small downward velocities so the controller stays glued to slopes.
        if (isGrounded && velocity.y < 0)
            velocity.y = GroundedSnapVelocity;
    }

    private void MovePlayer()
    {
        float targetSpeed = (sprinting && isGrounded) ? sprintSpeed * speedModifier : walkSpeed * speedModifier;

        Vector3 inputDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        float currentAccel = isGrounded ? acceleration : acceleration * airControl;

        currentMovement = moveInput.sqrMagnitude > MoveInputThreshold
            ? Vector3.MoveTowards(currentMovement, inputDirection, currentAccel * Time.deltaTime)
            : Vector3.MoveTowards(currentMovement, Vector3.zero, deceleration * Time.deltaTime);

        Vector3 finalMovement = currentMovement * targetSpeed + Vector3.up * velocity.y + windPush;
        controller.Move(finalMovement * Time.deltaTime);
    }

    private void HandleJump()
    {
        // v = sqrt(2gh) gives the upward velocity that reaches the jump height.
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ApplyGravity()
    {
        // Fall faster once descending so jumps feel responsive.
        velocity.y += (velocity.y < 0 ? gravity * fallMultiplier : gravity) * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    #endregion
}
