using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private Vector3 holdOffset = Vector3.zero;
    [SerializeField] private Vector3 holdRotationOffset = Vector3.zero;

    [Header("Physics")]
    [SerializeField, Range(0.5f, 3f)] private float throwForceMultiplier = 1.2f;
    [SerializeField, Range(0.1f, 1f)] private float dropVelocityDamping = 0.85f;
    [SerializeField] private bool disableColliderWhileHeld = true;
    [SerializeField] private bool preserveScaleOnPickup = true;

    private Rigidbody rb;
    private Collider[] colliders;

    private Transform originalParent;
    private Vector3 originalLocalScale;
    private bool isBeingHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>(true);
        originalLocalScale = transform.localScale;
    }

    public void Pickup(Transform holdPosition)
    {
        if (isBeingHeld || holdPosition == null) return;

        // Store original state
        originalParent = transform.parent;

        // Detach cleanly
        transform.SetParent(null, worldPositionStays: true);

        // Position & rotate relative to hold position
        transform.SetPositionAndRotation(holdPosition.position, holdPosition.rotation);
        transform.Translate(holdOffset, Space.Self);
        transform.Rotate(holdRotationOffset, Space.Self);

        if (!preserveScaleOnPickup)
            transform.localScale = originalLocalScale;

        // Physics setup
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (disableColliderWhileHeld)
            SetCollidersEnabled(false);

        isBeingHeld = true;
    }

    public void Drop()
    {
        if (!isBeingHeld) return;

        ReleaseObject();

        // Gentle drop
        rb.linearVelocity *= dropVelocityDamping;
        rb.AddForce(Vector3.down * 1.8f, ForceMode.Impulse);
    }

    public void Throw(Vector3 throwDirection, float baseForce = 12f)
    {
        if (!isBeingHeld || throwDirection.sqrMagnitude < 0.001f) return;

        ReleaseObject();

        float finalForce = baseForce * throwForceMultiplier * Mathf.Clamp(rb.mass, 0.2f, 5f);

        rb.AddForce(throwDirection.normalized * finalForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * (finalForce * 0.25f), ForceMode.Impulse);
    }

    private void ReleaseObject()
    {
        transform.SetParent(originalParent, worldPositionStays: true);

        rb.isKinematic = false;
        if (disableColliderWhileHeld)
            SetCollidersEnabled(true);

        isBeingHeld = false;
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }

    // Optional smooth physics follow (uncomment if you prefer non-kinematic carrying)
    /*
    private void FixedUpdate()
    {
        if (isBeingHeld && !rb.isKinematic && originalParent != null)
        {
            // Advanced physics-based carrying can go here
        }
    }
    */

    // Public API
    public bool IsBeingHeld => isBeingHeld;
    public Rigidbody Rigidbody => rb;
}