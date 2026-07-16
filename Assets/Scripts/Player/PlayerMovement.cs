using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private float fallMultiplier = 3.5f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.25f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private InputAction moveWASD;
    private InputAction moveArrows;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private Vector3 velocity;
    private bool sprinting;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveWASD = playerControls.FindActionMap("Move", true).FindAction("WASD", true);
        moveArrows = playerControls.FindActionMap("Move", true).FindAction("ArrowsKeys", true);
        jumpAction = playerControls.FindActionMap("Jump", true).FindAction("Jump", true);
        sprintAction = playerControls.FindActionMap("Sprint", true).FindAction("Run", true);
    }

    private void OnEnable()
    {
        moveWASD.Enable();
        moveArrows.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveWASD.Disable();
        moveArrows.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    private void Update()
    {
        CheckGround();
        HandleSprint();
        MovePlayer();
        ApplyGravity();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void HandleSprint()
    {
        sprinting = sprintAction.ReadValue<float>() > 0f;
    }

    private void MovePlayer()
    {
        Vector2 input = moveWASD.ReadValue<Vector2>() + moveArrows.ReadValue<Vector2>();
        float currentSpeed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 movement = transform.right * input.x + transform.forward * input.y;
        controller.Move(currentSpeed * Time.deltaTime * movement);
    }

    private void ApplyGravity()
    {
        // Reset vertical velocity when grounded to prevent accumulation
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Check for jump input while grounded
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply extra gravity when falling so the descent feels snappy, not floaty
        if (!isGrounded)
        {
            velocity.y += gravity * (fallMultiplier - 1f) * Time.deltaTime;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
