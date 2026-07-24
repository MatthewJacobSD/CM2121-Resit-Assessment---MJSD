using UnityEngine;
using UnityEngine.Events;

public class RecycleBinInteractable : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private ParticleSystem successVFX;
    [SerializeField] private ParticleSystem errorVFX;
    [SerializeField] private AudioSource successSFX;
    [SerializeField] private AudioSource errorSFX;

    [Header("Events")]
    public UnityEvent OnCorrectRecycle;
    public UnityEvent OnIncorrectRecycle;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
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
        int score = item.ScoreValue;
        bool isCorrect = score > 0;

        if (isCorrect)
        {
            ScoreManager.Instance.AddScore(score);
            TriggerFeedback(true, item.transform.position);
            OnCorrectRecycle?.Invoke();
        }
        else
        {
            ScoreManager.Instance.AddPenalty(Mathf.Abs(score));
            TriggerFeedback(false, item.transform.position);
            OnIncorrectRecycle?.Invoke();
        }

        GameManager.Instance.ReportRecycled(item.gameObject, score);
        Destroy(item.gameObject);
    }

    private void TriggerFeedback(bool success, Vector3 position)
    {
        if (success)
        {
            if (successVFX) Instantiate(successVFX, position, Quaternion.identity);
            if (successSFX) successSFX.Play();
        }
        else
        {
            if (errorVFX) Instantiate(errorVFX, position, Quaternion.identity);
            if (errorSFX) errorSFX.Play();
        }
    }
}