using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [Header("References (auto-created if empty)")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Text promptText;
    [SerializeField] private Text warningText;

    [Header("Settings")]
    [SerializeField] private float warningDuration = 2.5f;

    [Header("Prompt Style")]
    [SerializeField] private Color promptColor = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private int promptFontSize = 22;

    [Header("Warning Style")]
    [SerializeField] private Color warningColor = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private int warningFontSize = 20;

    private PlayerInteraction interaction;
    private float warningTimer;

    private void Awake()
    {
        interaction = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        if (uiCanvas == null)
            CreateUI();

        promptText.gameObject.SetActive(false);
        warningText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        interaction.OnTargetFound += ShowPickupPrompt;
        interaction.OnTargetLost += HidePickupPrompt;
        interaction.OnObjectPickedUp += ShowDropPrompt;
        interaction.OnObjectDropped += HideDropPrompt;
        interaction.OnWarningShown += ShowWarning;
    }

    private void OnDisable()
    {
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

    private void ShowPickupPrompt(PickupObject obj)
    {
        promptText.text = "Press [E] to Pick Up";
        promptText.gameObject.SetActive(true);
    }

    private void HidePickupPrompt()
    {
        promptText.gameObject.SetActive(false);
    }

    private void ShowDropPrompt(PickupObject obj)
    {
        promptText.text = "Press [Q] to Drop";
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

    private void CreateUI()
    {
        var canvasGO = new GameObject("InteractionCanvas");
        canvasGO.transform.SetParent(transform, false);

        uiCanvas = canvasGO.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 10;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        promptText = CreateText("PromptText", canvasGO.transform,
            "Press [E] to Pick Up",
            new Vector2(0, 120),
            promptFontSize,
            promptColor);

        warningText = CreateText("WarningText", canvasGO.transform,
            "",
            new Vector2(0, 170),
            warningFontSize,
            warningColor);

        warningText.gameObject.SetActive(false);
    }

    private Text CreateText(string name, Transform parent, string content,
        Vector2 anchoredPos, int fontSize, Color color)
    {
        var textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);

        var rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(500, 50);

        var outline = textGO.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        var text = textGO.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;

        return text;
    }
}
