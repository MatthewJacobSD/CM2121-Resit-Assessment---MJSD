using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Reference to the PlayerControls Input Action Asset.")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("References")]
    [Tooltip("Assign the Main Camera here.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Mouse Settings")]
    [SerializeField, Range(0.1f, 10f)]
    private float sensitivityX = 2f;

    [SerializeField, Range(0.1f, 10f)]
    private float sensitivityY = 2f;

    [SerializeField]
    private bool invertY = false;

    [Header("Vertical Clamp")]
    [SerializeField]
    private float minVerticalAngle = -85f;

    [SerializeField]
    private float maxVerticalAngle = 85f;


    // Input Action
    private InputAction lookAction;

    // Current mouse/controller input
    private Vector2 lookInput;

    // Current vertical camera rotation
    private float verticalRotation = 0f;


    private void Awake()
    {
        // Automatically use the Main Camera if none is assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Get the Look action from the Player Action Map
        lookAction = playerControls
            .FindActionMap("Player", true)
            .FindAction("Look", true);
    }


    private void OnEnable()
    {
        lookAction.Enable();
    }


    private void OnDisable()
    {
        lookAction.Disable();
    }


    private void Start()
    {
        // Lock the cursor to the centre of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        RotatePlayer();
    }


    /// <summary>
    /// Rotates the player horizontally and the camera vertically.
    /// </summary>
    private void RotatePlayer()
    {
        // Read mouse/controller input
        lookInput = lookAction.ReadValue<Vector2>();

        // Calculate rotation values
        float mouseX = lookInput.x * sensitivityX * Time.deltaTime * 100f;
        float mouseY = lookInput.y * sensitivityY * Time.deltaTime * 100f;

        // Optional inverted Y-axis
        if (invertY)
        {
            mouseY = -mouseY;
        }

        // Rotate the Player left and right (Yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the Camera up and down (Pitch)
        verticalRotation -= mouseY;

        // Prevent looking too far up or down
        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minVerticalAngle,
            maxVerticalAngle
        );

        cameraTransform.localRotation = Quaternion.Euler(
            verticalRotation,
            0f,
            0f
        );
    }
}