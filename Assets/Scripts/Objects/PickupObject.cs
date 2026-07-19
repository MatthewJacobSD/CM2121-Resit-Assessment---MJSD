using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

[RequireComponent(typeof(Collider))]
public class PickupObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider objectCollider;

    private Transform originalParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
    }

    private void Pickup(Transform holdPosition)
    {
        originalParent = transform.parent;

        transform.parent = holdPosition;

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        rb.isKinematic = true;
        objectCollider.enabled = false;
    }

    public void Drop()
    {
        transform.SetParent(originalParent);

        rb.isKinematic = false;
        objectCollider.enabled = true;
    }
}
