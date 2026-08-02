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
