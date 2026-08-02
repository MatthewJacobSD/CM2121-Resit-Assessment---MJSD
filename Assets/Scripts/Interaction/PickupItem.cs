using UnityEngine;

/// <summary>Types of recyclable items that can be picked up and sorted.</summary>
public enum ItemType { Plant, Toy, Bottle }

/// <summary>
/// A carryable recyclable item: manages its state while picked up, dropped or
/// thrown, toggling physics and colliders accordingly.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    #region Serialized Fields

    [Header("Item Info")]
    [SerializeField] private ItemType itemType = ItemType.Plant;
    [SerializeField] private string itemName = "Item";

    [Header("Base Scoring")]
    [SerializeField] private int baseScore = 10;

    [Header("Physics")]
    [Tooltip("Fraction of velocity kept after a drop, softening the release.")]
    [SerializeField] private float dropVelocityDamping = 0.85f;
    [Tooltip("Disables colliders while held so the item does not collide with the player.")]
    [SerializeField] private bool disableColliderWhileHeld = true;

    #endregion

    #region Private Fields

    private Rigidbody rb;
    private Collider[] colliders;
    private Transform originalParent;
    private bool isBeingHeld;

    #endregion

    #region Public Properties

    /// <summary>Whether the item is currently carried by the player.</summary>
    public bool IsBeingHeld => isBeingHeld;

    /// <summary>The item's recycle category.</summary>
    public ItemType ItemType => itemType;

    /// <summary>Display name of the item.</summary>
    public string ItemName => itemName;

    /// <summary>Base score awarded for recycling this item correctly.</summary>
    public int BaseScore => baseScore;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>(true);
    }

    #endregion

    #region Public Methods

    /// <summary>Attaches the item to the hold position and disables physics.</summary>
    public void Pickup(Transform holdPosition)
    {
        if (isBeingHeld) return;

        originalParent = transform.parent;
        transform.SetParent(holdPosition, false);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        rb.isKinematic = true;
        if (disableColliderWhileHeld) SetCollidersEnabled(false);

        isBeingHeld = true;
    }

    /// <summary>Releases the item back into the world, keeping a little velocity.</summary>
    public void Drop()
    {
        if (!isBeingHeld) return;

        transform.SetParent(originalParent, true);
        rb.isKinematic = false;
        rb.linearVelocity *= dropVelocityDamping;

        if (disableColliderWhileHeld) SetCollidersEnabled(true);
        isBeingHeld = false;
    }

    /// <summary>Throws the item with an impulse force in the given direction.</summary>
    public void Throw(Vector3 direction, float force)
    {
        if (!isBeingHeld) return;

        transform.SetParent(originalParent, true);
        rb.isKinematic = false;
        if (disableColliderWhileHeld) SetCollidersEnabled(true);

        // Clear any leftover motion so the throw is predictable.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        isBeingHeld = false;
    }

    #endregion

    #region Private Methods

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider col in colliders)
        {
            if (col != null) col.enabled = enabled;
        }
    }

    #endregion
}
