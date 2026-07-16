using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float minimumLookAngle = -85f;
    [SerializeField] private float maximumLookAngle = 85f;

    private InputAction lookAction;
    private float verticalRotation;

    private void Awake()
    {
        // Grab the mouse look action before OnEnable tries to use it
        lookAction = playerControls.FindActionMap("Look", true).FindAction("Search", true);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    private void Update()
    {
        LookAround();
    }

    // Read mouse delta from the input system and rotate the camera
    private void LookAround()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Rotate camera up/down and clamp to prevent flipping
        verticalRotation -= lookInput.y * sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, minimumLookAngle, maximumLookAngle);
        
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        
        // Rotate the player left/right so the whole body turns
        transform.Rotate(lookInput.x * sensitivity * Time.deltaTime * Vector3.up);
    }
}
