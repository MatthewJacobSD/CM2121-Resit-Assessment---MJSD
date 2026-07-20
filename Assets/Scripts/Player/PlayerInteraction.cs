using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Raycast")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Pickup")]
    [SerializeField] private Transform holdPosition;
    [SerializeField] private float throwForce = 12f;

    private PickupObject currentHeldObject;
    private InputAction interactAction;
    private InputAction dropAction;

    private void Awake()
    {
        var playerMap = playerControls.FindActionMap("Player", true);
        interactAction = playerMap.FindAction("Interact", true);
        dropAction = playerMap.FindAction("Drop", true);
    }

    private void OnEnable()
    {
        interactAction.performed += OnInteract;
        dropAction.performed += OnDrop;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        dropAction.performed -= OnDrop;
    }

    private void Update()
    {
        HighlightInteractable();
    }

    private void HighlightInteractable()
    {
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent<PickupObject>(out var pickup))
            {
                Debug.DrawRay(rayOrigin.position, rayOrigin.forward * hit.distance, Color.cyan);
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject != null) return;

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.TryGetComponent<PickupObject>(out var pickup))
            {
                currentHeldObject = pickup;
                currentHeldObject.Pickup(holdPosition);
            }
        }
    }

    private void OnDrop(InputAction.CallbackContext ctx)
    {
        if (currentHeldObject == null) return;
        currentHeldObject.Drop();
        currentHeldObject = null;
    }

    public void ThrowHeldObject()
    {
        if (currentHeldObject == null) return;
        currentHeldObject.Drop();
        if (currentHeldObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(rayOrigin.forward * throwForce, ForceMode.Impulse);
        }
        currentHeldObject = null;
    }
}
