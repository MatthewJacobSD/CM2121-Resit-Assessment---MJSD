using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>Bin categories an item can be recycled into.</summary>
public enum BinType { NatureRecycling, PlasticRecycling, GeneralWaste }

/// <summary>
/// A recycling bin: accepts thrown items, checks whether they belong in this
/// bin, plays success/error feedback and reports the result to the game.
/// </summary>
public class RecycleBinInteractable : MonoBehaviour
{
    #region Serialized Fields

    [Header("Bin Settings")]
    [SerializeField] private BinType binType = BinType.NatureRecycling;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem successVFX;
    [SerializeField] private ParticleSystem errorVFX;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;

    #endregion

    #region Private Fields

    private AudioSource audioSource;

    #endregion

    #region Public Properties

    /// <summary>Which kind of waste this bin accepts.</summary>
    public BinType BinType => binType;

    #endregion

    #region Events

    public UnityEvent OnCorrectRecycle;
    public UnityEvent OnIncorrectRecycle;

    /// <summary>Invoked with true/false when an item was processed correctly or not.</summary>
    public event Action<bool> OnItemProcessed;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Bins are trigger volumes so items pass through and trigger processing.
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;

        // Add a solid (non-trigger) collider so the player cannot walk through
        // the bin body. The solid is shorter than the trigger so thrown items
        // entering from above still reach the trigger volume.
        EnsureSolidCollider();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.IsPlaying) return;

        PickupItem item = other.GetComponentInParent<PickupItem>();
        if (item == null || item.IsBeingHeld) return;

        ProcessItem(item);
    }

    #endregion

    #region Public Methods

    /// <summary>Returns whether the given item type belongs in this bin.</summary>
    public bool AcceptsItem(ItemType itemType)
    {
        return CalculateScore(itemType, binType) > 0;
    }

    #endregion

    #region Private Methods

    private void ProcessItem(PickupItem item)
    {
        bool isCorrect = AcceptsItem(item.ItemType);

        if (isCorrect)
        {
            if (successVFX) Instantiate(successVFX, item.transform.position, Quaternion.identity);
            if (successClip && audioSource) audioSource.PlayOneShot(successClip);
            OnCorrectRecycle?.Invoke();
        }
        else
        {
            if (errorVFX) Instantiate(errorVFX, item.transform.position, Quaternion.identity);
            if (errorClip && audioSource) audioSource.PlayOneShot(errorClip);
            OnIncorrectRecycle?.Invoke();
        }

        GameManager.Instance.ReportRecycled(item.gameObject, CalculateScore(item.ItemType, binType));
        OnItemProcessed?.Invoke(isCorrect);
        Destroy(item.gameObject);
    }

    /// <summary>
    /// Ensures a solid (non-trigger) BoxCollider exists on the bin for physical
    /// blocking. This collider is always active — the player collides with it
    /// regardless of held item. It is shorter than the trigger so thrown items
    /// entering from above still reach the trigger volume.
    /// </summary>
    private void EnsureSolidCollider()
    {
        // Check if a non-trigger collider already exists.
        foreach (var c in GetComponents<Collider>())
        {
            if (!c.isTrigger)
                return;
        }

        // No solid collider found — add one covering the bin body.
        // The existing trigger (1.28 × 1.89 × 1.20, center 0,1,0.23) covers
        // the full bin including the opening. The solid is shorter, leaving
        // the top open for thrown items to reach the trigger.
        BoxCollider solid = gameObject.AddComponent<BoxCollider>();
        solid.isTrigger = false;
        solid.size = new Vector3(1.2f, 1.5f, 1.1f);
        solid.center = new Vector3(0f, 0.75f, 0.23f);
    }

    // Scoring matrix: positive values are correct matches, negative are penalties.
    private int CalculateScore(ItemType itemType, BinType bin)
    {
        return (itemType, bin) switch
        {
            (ItemType.Plant, BinType.NatureRecycling) => 20,
            (ItemType.Bottle, BinType.NatureRecycling) => -15,
            (ItemType.Toy, BinType.NatureRecycling) => -25,

            (ItemType.Bottle, BinType.PlasticRecycling) => 20,
            (ItemType.Plant, BinType.PlasticRecycling) => -45,
            (ItemType.Toy, BinType.PlasticRecycling) => -15,

            (ItemType.Toy, BinType.GeneralWaste) => 25,
            (ItemType.Bottle, BinType.GeneralWaste) => 15,
            (ItemType.Plant, BinType.GeneralWaste) => -20,

            _ => -10
        };
    }

    #endregion
}
