using UnityEngine;
using UnityEngine.Events;

public class RecycleBinInteractable : MonoBehaviour
{
    [Header("Bin Type")]
    [SerializeField] private ItemType acceptedType = ItemType.Plant;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem successVFX;
    [SerializeField] private ParticleSystem errorVFX;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;

    [Header("Events")]
    public UnityEvent OnCorrectRecycle;
    public UnityEvent OnIncorrectRecycle;

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
        bool isCorrect = item.ItemType == acceptedType;

        if (isCorrect)
        {
            if (successVFX) Instantiate(successVFX, item.transform.position, Quaternion.identity);
            if (successClip) audioSource.PlayOneShot(successClip);
            OnCorrectRecycle?.Invoke();
        }
        else
        {
            if (errorVFX) Instantiate(errorVFX, item.transform.position, Quaternion.identity);
            if (errorClip) audioSource.PlayOneShot(errorClip);
            OnIncorrectRecycle?.Invoke();
        }

        GameManager.Instance.ReportRecycled(item.gameObject, item.ScoreValue);
        Destroy(item.gameObject);
    }
}
