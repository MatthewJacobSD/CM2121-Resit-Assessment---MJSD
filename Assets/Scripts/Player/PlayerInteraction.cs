using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interactions: raycast targeting of pickups, smooth carry of a
/// held item, and drop/aim/throw actions. Emits events for UI feedback.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region Constants

    private const float ThrowChargeRate = 15f;
    private const float DropUpwardNudge = 0.5f;

    #endregion

    #region Serialized Fields

    [Header("Input")]
    [Tooltip("Input Action Asset containing the Player action map.")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Raycast")]
    [Tooltip("Transform the interaction ray is cast from (usually the camera).")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Holding")]
    [Tooltip("Position the held item smoothly follows while carried.")]
    [SerializeField] private Transform holdPosition;
    [SerializeField] private float holdFollowSpeed = 15f;
    [SerializeField] private float holdRotationSpeed = 12f;
    [SerializeField] private float dropForwardForce = 1.5f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float maxThrowForce = 20f;
    [Tooltip("Offsets the held item forward/up while aiming to avoid the camera.")]
    [SerializeField] private float aimForwardOffset = 0.5f;
    [SerializeField] private float aimDownOffset = 0.15f;

    #endregion

    #region Private Fields

    private InputAction interactAction;
    private InputAction dropAction;
    private InputAction aimAction;
    private InputAction throwAction;

    private PickupItem currentHeldObject;
    private PickupItem currentTarget;

    private bool isAiming;
    private float currentThrowForce;

    #endregion

    #region Public Properties

    /// <summary>The item currently carried by the player, or null.</summary>
    public PickupItem CurrentHeldObject => currentHeldObject;

    /// <summary>The pickup currently under the interaction crosshair, or null.</summary>
    public PickupItem CurrentTarget => currentTarget;

    #endregion

    #region Events

    public event System.Action<PickupItem> OnTargetFound;
    public event System.Action OnTargetLost;
    public event System.Action<PickupItem> OnObjectPickedUp;
    public event System.Action OnObjectDropped;
    public event System.Action<string> OnWarningShown;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InputActionMap playerMap = playerControls.FindActionMap("Player", true);

        interactAction = playerMap.FindAction("Interact", true);
        dropAction = playerMap.FindAction("Drop", true);
        aimAction = playerMap.FindAction("Aim", true);
        throwAction = playerMap.FindAction("Throw", true);

        currentThrowForce = throwForce;
    }

    private void OnEnable()
    {
        interactAction.Enable();
        dropAction.Enable();
        aimAction.Enable();
        throwAction.Enable();

        interactAction.performed += OnInteract;
        dropAction.performed += OnDrop;
        aimAction.performed += OnAimStarted;
        aimAction.canceled += OnAimStopped;
        throwAction.performed += OnThrow;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        dropAction.performed -= OnDrop;
        aimAction.performed -= OnAimStarted;
        aimAction.canceled -= OnAimStopped;
        throwAction.performed -= OnThrow;

        interactAction.Disable();
        dropAction.Disable();
        aimAction.Disable();
        throwAction.Disable();
    }

    private void Update()
    {
        CheckTarget();
        FollowHoldPosition();

        // Holding aim + throw builds up throw power until released.
        if (isAiming && currentHeldObject != null && throwAction.IsPressed())
        {
            currentThrowForce = Mathf.Clamp(currentThrowForce + ThrowChargeRate * Time.deltaTime, throwForce, maxThrowForce);
        }
    }

    #endregion

    #region Interaction Logic

    private void CheckTarget()
    {
        // No targeting while an item is being carried.
        if (currentHeldObject != null)
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                OnTargetLost?.Invoke();
            }
            return;
        }

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent(out PickupItem pickup) && !pickup.IsBeingHeld)
            {
                if (currentTarget != pickup)
                {
                    currentTarget = pickup;
                    OnTargetFound?.Invoke(pickup);
                }
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget = null;
            OnTargetLost?.Invoke();
        }
    }

    private void FollowHoldPosition()
    {
        if (currentHeldObject == null || holdPosition == null) return;

        Transform objTransform = currentHeldObject.transform;
        Vector3 targetPos = holdPosition.position;

        // Move the item slightly forward/down while aiming so it clears the camera.
        if (isAiming)
        {
            targetPos += rayOrigin.forward * aimForwardOffset + Vector3.down * aimDownOffset;
        }

        objTransform.SetPositionAndRotation(
            Vector3.Lerp(objTransform.position, targetPos, holdFollowSpeed * Time.deltaTime),
            Quaternion.Slerp(objTransform.rotation, holdPosition.rotation, holdRotationSpeed * Time.deltaTime)
        );
    }

    #endregion

    #region Input Handlers

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject != null)
        {
            OnWarningShown?.Invoke("Already carrying an item! Press Q to drop it first.");
            return;
        }

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent(out PickupItem pickup) && !pickup.IsBeingHeld)
            {
                currentHeldObject = pickup;
                currentHeldObject.Pickup(holdPosition);
                currentTarget = null;
                AudioManager.Instance?.PlayPickupSFX();
                OnObjectPickedUp?.Invoke(currentHeldObject);
                OnTargetLost?.Invoke();
            }
        }
    }

    private void OnDrop(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject == null) return;

        PickupItem dropped = currentHeldObject;
        dropped.Drop();

        if (dropped.TryGetComponent(out Rigidbody rb))
        {
            // Cancel leftover velocity so the item falls straight down with a nudge.
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(rayOrigin.forward * dropForwardForce + Vector3.up * DropUpwardNudge, ForceMode.Impulse);
        }

        currentHeldObject = null;
        isAiming = false;
        AudioManager.Instance?.PlayDropSFX();
        OnObjectDropped?.Invoke();
    }

    private void OnThrow(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject == null || !isAiming) return;

        currentHeldObject.Throw(rayOrigin.forward, currentThrowForce);

        currentHeldObject = null;
        isAiming = false;
        currentThrowForce = throwForce;
        AudioManager.Instance?.PlayDropSFX();
        OnObjectDropped?.Invoke();
    }

    private void OnAimStarted(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject != null)
            isAiming = true;
    }

    private void OnAimStopped(InputAction.CallbackContext ctx)
    {
        isAiming = false;
    }

    #endregion
}
