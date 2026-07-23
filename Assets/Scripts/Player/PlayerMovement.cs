using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;


    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8.5f;

    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 25f;

    [Range(0f, 1f)]
    [SerializeField] private float airControl = 0.6f;


    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.8f;

    [SerializeField] private float gravity = -18f;

    [SerializeField] private float fallMultiplier = 2.5f;


    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundDistance = 0.2f;

    [SerializeField] private LayerMask groundMask;

    // Components
    private CharacterController controller;

    // Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    // Movement variables
    private Vector2 moveInput;

    private Vector3 velocity;

    private Vector3 currentMovement;


    private bool isGrounded;

    private bool sprinting;

    public bool IsSprinting => sprinting;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Get actions from Input System
        InputActionMap playerMap =
            playerControls.FindActionMap("Player", true);

        moveAction =
            playerMap.FindAction("Movement", true);

        jumpAction =
            playerMap.FindAction("Jump", true);

        sprintAction =
            playerMap.FindAction("Sprint", true);
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

    /// <summary>
    /// Reads movement, sprint input.
    /// </summary>
    private void ReadInput()
    {
        moveInput =
            moveAction.ReadValue<Vector2>();

        // Prevent faster diagonal movement
        moveInput =
            Vector2.ClampMagnitude(moveInput, 1f);

        sprinting =
            sprintAction.ReadValue<float>() > 0.5f;
    }

    /// <summary>
    /// Checks if player is touching the ground.
    /// </summary>
    private void CheckGround()
    {
        isGrounded =
            Physics.CheckSphere(
                groundCheck.position,
                groundDistance,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

        if (isGrounded && velocity.y < 0)
        {
            // Keeps player grounded
            velocity.y = -2f;
        }
    }

    /// <summary>
    /// Handles walking and sprinting.
    /// </summary>
    private void MovePlayer()
    {
        float targetSpeed =
            sprinting && isGrounded
            ? sprintSpeed
            : walkSpeed;

        // Movement based on where the player is looking
        Vector3 inputDirection =
            (
                transform.forward * moveInput.y +
                transform.right * moveInput.x
            ).normalized;

        float currentAcceleration =
            isGrounded
            ? acceleration
            : acceleration * airControl;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            currentMovement =
                Vector3.MoveTowards(
                    currentMovement,
                    inputDirection,
                    currentAcceleration * Time.deltaTime
                );
        }
        else
        {
            currentMovement =
                Vector3.MoveTowards(
                    currentMovement,
                    Vector3.zero,
                    deceleration * Time.deltaTime
                );
        }

        Vector3 horizontalVelocity =
            currentMovement * targetSpeed;

        Vector3 finalMovement =
            horizontalVelocity +
            Vector3.up * velocity.y;

        controller.Move(
            finalMovement * Time.deltaTime
        );
    }

    /// <summary>
    /// Handles jumping.
    /// </summary>
    private void HandleJump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y =
                Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );
        }
    }

    /// <summary>
    /// Applies gravity with faster falling.
    /// </summary>
    private void ApplyGravity()
    {
        if (velocity.y < 0)
        {
            // Faster fall
            velocity.y +=
                gravity *
                fallMultiplier *
                Time.deltaTime;
        }
        else
        {
            // Normal jump ascent
            velocity.y +=
                gravity *
                Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color =
            isGrounded
            ? Color.green
            : Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}