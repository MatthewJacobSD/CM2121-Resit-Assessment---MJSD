using UnityEngine;
using UnityEngine.Events;
using System;

public enum BinType { NatureRecycling, PlasticRecycling, GeneralWaste }

public class RecycleBinInteractable : MonoBehaviour
{
    [Header("Bin Settings")]
    [SerializeField] private BinType binType = BinType.NatureRecycling;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem successVFX;
    [SerializeField] private ParticleSystem errorVFX;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;

    [Header("Events")]
    public UnityEvent OnCorrectRecycle;
    public UnityEvent OnIncorrectRecycle;

    public event Action<bool> OnItemProcessed;
    public BinType BinType => binType;

    private AudioSource audioSource;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.IsPlaying) return;

        var item = other.GetComponentInParent<PickupItem>();
        if (item == null || item.IsBeingHeld) return;

        ProcessItem(item);
    }

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

    public bool AcceptsItem(ItemType itemType)
    {
        return CalculateScore(itemType, binType) > 0;
    }

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
}
