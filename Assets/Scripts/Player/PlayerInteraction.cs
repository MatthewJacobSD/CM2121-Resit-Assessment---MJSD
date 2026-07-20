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

    private InputAction interactAction;
    private InputAction dropAction;

    private PickupObject currentHeldObject;
    private PickupObject currentTarget;

    public PickupObject CurrentHeldObject => currentHeldObject;
    public PickupObject CurrentTarget => currentTarget;

    public event System.Action<PickupObject> OnTargetFound;
    public event System.Action OnTargetLost;
    public event System.Action<PickupObject> OnObjectPickedUp;
    public event System.Action OnObjectDropped;
    public event System.Action<string> OnWarningShown;

    private void Awake()
    {
        var playerMap = playerControls.FindActionMap("Player", true);
        interactAction = playerMap.FindAction("Interact", true);
        dropAction = playerMap.FindAction("Drop", true);
    }

    private void OnEnable()
    {
        interactAction.Enable();
        dropAction.Enable();
        interactAction.performed += OnInteract;
        dropAction.performed += OnDrop;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        dropAction.performed -= OnDrop;
        interactAction.Disable();
        dropAction.Disable();
    }

    private void Update()
    {
        CheckTarget();
        FollowHoldPosition();
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

        if (Physics.Raycast(
            rayOrigin.position,
            rayOrigin.forward,
            out RaycastHit hit,
            interactDistance,
            interactLayer))
        {
            if (hit.collider.TryGetComponent<PickupObject>(out var pickup) && !pickup.IsBeingHeld)
            {
                if (currentTarget != pickup)
                {
                    currentTarget = pickup;
                    OnTargetFound?.Invoke(pickup);
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    currentTarget = null;
                    OnTargetLost?.Invoke();
                }
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                OnTargetLost?.Invoke();
            }
        }
    }

    private void FollowHoldPosition()
    {
        if (currentHeldObject == null || holdPosition == null) return;

        var objTransform = currentHeldObject.transform;

        objTransform.position = Vector3.Lerp(
            objTransform.position,
            holdPosition.position,
            holdFollowSpeed * Time.deltaTime
        );

        objTransform.rotation = Quaternion.Slerp(
            objTransform.rotation,
            holdPosition.rotation,
            holdRotationSpeed * Time.deltaTime
        );
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject != null)
        {
            OnWarningShown?.Invoke("Already carrying an item! Press Q to drop it first.");
            return;
        }

        if (Physics.Raycast(
            rayOrigin.position,
            rayOrigin.forward,
            out RaycastHit hit,
            interactDistance,
            interactLayer))
        {
            if (hit.collider.TryGetComponent<PickupObject>(out var pickup) && !pickup.IsBeingHeld)
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

        var dropped = currentHeldObject;
        dropped.Drop();

        if (dropped.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(
                (rayOrigin.forward * dropForwardForce) + (Vector3.up * 0.5f),
                ForceMode.Impulse
            );
        }

        currentHeldObject = null;
        OnObjectDropped?.Invoke();
    }
}
