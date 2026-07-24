using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField, Range(0.1f, 10f)] private float sensitivityX = 2f;
    [SerializeField, Range(0.1f, 10f)] private float sensitivityY = 2f;
    [SerializeField] private bool invertY = false;

    [SerializeField] private float minVerticalAngle = -85f;
    [SerializeField] private float maxVerticalAngle = 85f;

    private InputAction lookAction;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lookAction = playerControls.FindActionMap("Player", true).FindAction("Look", true);
    }

    private void OnEnable() => lookAction.Enable();
    private void OnDisable() => lookAction.Disable();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivityX * Time.deltaTime * 100f;
        float mouseY = lookInput.y * sensitivityY * Time.deltaTime * 100f * (invertY ? -1 : 1);

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}