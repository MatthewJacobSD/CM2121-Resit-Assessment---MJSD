using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Raycast")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Holding")]
    [SerializeField] private Transform holdPosition;
    [SerializeField] private float holdFollowSpeed = 15f;
    [SerializeField] private float holdRotationSpeed = 12f;
    [SerializeField] private float dropForwardForce = 1.5f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float maxThrowForce = 20f;
    [SerializeField] private float aimForwardOffset = 0.5f;
    [SerializeField] private float aimDownOffset = 0.15f;

    private InputAction interactAction;
    private InputAction dropAction;
    private InputAction aimAction;
    private InputAction throwAction;

    private PickupItem currentHeldObject;
    private PickupItem currentTarget;

    private bool isAiming;
    private float currentThrowForce;

    public PickupItem CurrentHeldObject => currentHeldObject;
    public PickupItem CurrentTarget => currentTarget;

    public event System.Action<PickupItem> OnTargetFound;
    public event System.Action OnTargetLost;
    public event System.Action<PickupItem> OnObjectPickedUp;
    public event System.Action OnObjectDropped;
    public event System.Action<string> OnWarningShown;

    private void Awake()
    {
        var playerMap = playerControls.FindActionMap("Player", true);

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

        if (isAiming && currentHeldObject != null && throwAction.IsPressed())
        {
            currentThrowForce = Mathf.Clamp(currentThrowForce + 15f * Time.deltaTime, throwForce, maxThrowForce);
        }
    }

    private void CheckTarget()
    {
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

        if (isAiming)
        {
            targetPos += rayOrigin.forward * aimForwardOffset + Vector3.down * aimDownOffset;
        }

        objTransform.SetPositionAndRotation(
            Vector3.Lerp(objTransform.position, targetPos, holdFollowSpeed * Time.deltaTime),
            Quaternion.Slerp(objTransform.rotation, holdPosition.rotation, holdRotationSpeed * Time.deltaTime)
        );
    }

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
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(rayOrigin.forward * dropForwardForce + Vector3.up * 0.5f, ForceMode.Impulse);
        }

        currentHeldObject = null;
        isAiming = false;
        OnObjectDropped?.Invoke();
    }

    private void OnThrow(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject == null || !isAiming) return;

        currentHeldObject.Throw(rayOrigin.forward, currentThrowForce);

        currentHeldObject = null;
        isAiming = false;
        currentThrowForce = throwForce;
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
}