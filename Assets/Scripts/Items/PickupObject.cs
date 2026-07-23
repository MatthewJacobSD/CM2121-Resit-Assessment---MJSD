using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupObject : MonoBehaviour
{
    [Header("Scoring")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private string itemName = "Item";

    [Header("Physics")]
    [SerializeField] private float dropVelocityDamping = 0.85f;
    [SerializeField] private bool disableColliderWhileHeld = true;

    private Rigidbody rb;
    private Collider[] colliders;
    private Transform originalParent;
    private bool isBeingHeld;

    public bool IsBeingHeld => isBeingHeld;
    public int ScoreValue => scoreValue;
    public string ItemName => itemName;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>(true);
    }

    public void Pickup(Transform holdPosition)
    {
        if (isBeingHeld) return;

        originalParent = transform.parent;

        transform.SetParent(holdPosition, false);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        rb.isKinematic = true;

        if (disableColliderWhileHeld)
            SetCollidersEnabled(false);

        isBeingHeld = true;
    }

    public void Drop()
    {
        if (!isBeingHeld) return;

        transform.SetParent(originalParent, worldPositionStays: true);

        rb.isKinematic = false;
        rb.linearVelocity *= dropVelocityDamping;

        if (disableColliderWhileHeld)
            SetCollidersEnabled(true);

        isBeingHeld = false;
    }

    public void Throw(Vector3 direction, float force)
    {
        if (!isBeingHeld) return;

        transform.SetParent(originalParent, worldPositionStays: true);

        rb.isKinematic = false;

        if (disableColliderWhileHeld)
            SetCollidersEnabled(true);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

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
}
