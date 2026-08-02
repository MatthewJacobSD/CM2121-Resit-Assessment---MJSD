using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows contextual interaction prompts ("Press E to Pick Up", drop/warning
/// messages) by listening to events raised by the player's interaction system.
/// </summary>
public class InteractionPromptUI : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private Text promptText;
    [SerializeField] private Text warningText;

    [Header("Settings")]
    [SerializeField] private float warningDuration = 2.5f;

    #endregion

    #region Private Fields

    private PlayerInteraction interaction;
    private float warningTimer;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        interaction = GetComponentInParent<PlayerInteraction>();
        if (interaction == null)
            interaction = FindAnyObjectByType<PlayerInteraction>();
    }

    private void Start()
    {
        promptText.gameObject.SetActive(false);
        warningText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (interaction == null) return;

        interaction.OnTargetFound += ShowPickupPrompt;
        interaction.OnTargetLost += HidePickupPrompt;
        interaction.OnObjectPickedUp += ShowDropPrompt;
        interaction.OnObjectDropped += HideDropPrompt;
        interaction.OnWarningShown += ShowWarning;
    }

    private void OnDisable()
    {
        if (interaction == null) return;

        interaction.OnTargetFound -= ShowPickupPrompt;
        interaction.OnTargetLost -= HidePickupPrompt;
        interaction.OnObjectPickedUp -= ShowDropPrompt;
        interaction.OnObjectDropped -= HideDropPrompt;
        interaction.OnWarningShown -= ShowWarning;
    }

    private void Update()
    {
        if (warningTimer > 0)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0)
                warningText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Prompt Display

    private void ShowPickupPrompt(PickupItem obj)
    {
        promptText.text = "Press [E] to Pick Up";
        promptText.gameObject.SetActive(true);
    }

    private void HidePickupPrompt()
    {
        promptText.gameObject.SetActive(false);
    }

    private void ShowDropPrompt(PickupItem obj)
    {
        promptText.text = "Press [Q] to Drop / Hold & Throw";
        promptText.gameObject.SetActive(true);
    }

    private void HideDropPrompt()
    {
        promptText.gameObject.SetActive(false);
    }

    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);
        warningTimer = warningDuration;
    }

    #endregion
}
