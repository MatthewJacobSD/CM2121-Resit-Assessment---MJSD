using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
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
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

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

    public bool IsSprinting => sprinting;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
    public float SpeedModifier => speedModifier;
    public float CurrentWalkSpeed => walkSpeed * speedModifier;
    public float CurrentSprintSpeed => sprintSpeed * speedModifier;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        var playerMap = playerControls.FindActionMap("Player", true);
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

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = Mathf.Clamp(modifier, 0.1f, 2f);
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
        sprinting = sprintAction.ReadValue<float>() > 0.5f;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void MovePlayer()
    {
        float targetSpeed = (sprinting && isGrounded) ? sprintSpeed * speedModifier : walkSpeed * speedModifier;

        Vector3 inputDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

        float currentAccel = isGrounded ? acceleration : acceleration * airControl;

        currentMovement = moveInput.sqrMagnitude > 0.01f
            ? Vector3.MoveTowards(currentMovement, inputDirection, currentAccel * Time.deltaTime)
            : Vector3.MoveTowards(currentMovement, Vector3.zero, deceleration * Time.deltaTime);

        Vector3 finalMovement = currentMovement * targetSpeed + Vector3.up * velocity.y;
        controller.Move(finalMovement * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ApplyGravity()
    {
        velocity.y += (velocity.y < 0 ? gravity * fallMultiplier : gravity) * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
