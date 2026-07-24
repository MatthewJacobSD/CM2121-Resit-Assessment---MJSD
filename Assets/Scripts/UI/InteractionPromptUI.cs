using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text promptText;
    [SerializeField] private Text warningText;

    [Header("Settings")]
    [SerializeField] private float warningDuration = 2.5f;

    private PlayerInteraction interaction;
    private float warningTimer;

    private void Awake()
    {
        interaction = GetComponentInParent<PlayerInteraction>() ?? FindAnyObjectByType<PlayerInteraction>();
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
}