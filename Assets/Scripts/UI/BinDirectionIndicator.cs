using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Lightweight GPS-style guide: while the player carries an item it shows the
/// nearest bin that accepts it, with a directional marker pinned to the screen
/// edge and the straight-line distance. Reuses the existing bin scoring logic
/// so it always points at the "correct" bin for the held item.
/// </summary>
public class BinDirectionIndicator : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [Tooltip("Label drawn on the HUD. Created automatically if left empty.")]
    [SerializeField] private TextMeshProUGUI indicatorText;

    [Header("Settings")]
    [Tooltip("Distance in screen pixels from the screen edge for the marker.")]
    [SerializeField] private float edgeMargin = 40f;
    [Tooltip("Only show once the player is within this many units of the bin.")]
    [SerializeField] private float maxGuideDistance = 60f;

    [Header("Nearby Message")]
    [Tooltip("Centre-screen message shown when a bin is nearby. Created automatically if left empty.")]
    [SerializeField] private TextMeshProUGUI nearbyMessageText;
    [Tooltip("Show the centre message within this many units of the target bin.")]
    [SerializeField] private float nearbyRadius = 10f;
    [Tooltip("Text displayed in the centre of the screen when the bin is nearby.")]
    [SerializeField] private string nearbyMessage = "This bin is nearby";

    #endregion

    #region Private Fields

    private PlayerInteraction interaction;
    private Camera playerCamera;
    private List<RecycleBinInteractable> bins;
    private int binsRefreshTimer;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        interaction = FindAnyObjectByType<PlayerInteraction>();
        playerCamera = Camera.main;
        bins = new List<RecycleBinInteractable>();
        CollectBins();
    }

    private void Start()
    {
        EnsureIndicatorText();
        EnsureNearbyMessageText();
        if (indicatorText != null)
            indicatorText.gameObject.SetActive(false);
        if (nearbyMessageText != null)
            nearbyMessageText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Re-scan occasionally so bins added/removed later are still found.
        if (binsRefreshTimer-- <= 0)
        {
            binsRefreshTimer = 60;
            CollectBins();
        }

        UpdateIndicator();
    }

    #endregion

    #region Indicator Logic

    private void CollectBins()
    {
        bins.Clear();
        bins.AddRange(FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None));
    }

    private void UpdateIndicator()
    {
        PickupItem held = interaction != null ? interaction.CurrentHeldObject : null;

        if (held == null || !GameManager.Instance.IsPlaying)
        {
            if (indicatorText != null)
                indicatorText.gameObject.SetActive(false);
            if (nearbyMessageText != null)
                nearbyMessageText.gameObject.SetActive(false);
            return;
        }

        RecycleBinInteractable best = FindNearestAcceptingBin(held.ItemType);
        if (best == null)
        {
            if (indicatorText != null)
                indicatorText.gameObject.SetActive(false);
            if (nearbyMessageText != null)
                nearbyMessageText.gameObject.SetActive(false);
            return;
        }

        Vector3 toBin = best.transform.position - (playerCamera != null ? playerCamera.transform.position : transform.position);
        float distance = toBin.magnitude;

        // Centre-screen "nearby" message when the player is close to the bin.
        if (nearbyMessageText != null)
        {
            bool showNearby = distance <= nearbyRadius;
            if (showNearby)
                nearbyMessageText.text = $"{nearbyMessage} ({distance:0}m)";
            nearbyMessageText.gameObject.SetActive(showNearby);
        }

        if (distance > maxGuideDistance)
        {
            if (indicatorText != null)
                indicatorText.gameObject.SetActive(false);
            return;
        }

        // Position the label at the screen edge in the bin's direction.
        Vector3 viewport = playerCamera != null
            ? playerCamera.WorldToViewportPoint(best.transform.position)
            : Vector3.zero;

        bool behind = viewport.z < 0f;
        Vector3 screenPos;
        if (behind)
        {
            // Mirror behind-the-camera targets onto the nearest edge.
            screenPos = new Vector3(viewport.x > 0.5f ? 0f : 1f, 0.5f, 0f);
        }
        else
        {
            screenPos = viewport;
            screenPos.x = Mathf.Clamp(screenPos.x, 0f, 1f);
            screenPos.y = Mathf.Clamp(screenPos.y, 0f, 1f);
        }

        RectTransform rect = indicatorText.rectTransform;
        float halfWidth = rect.rect.width * 0.5f;
        float halfHeight = rect.rect.height * 0.5f;

        Vector2 screenPx = new Vector2(
            Mathf.Clamp(screenPos.x * Screen.width, edgeMargin + halfWidth, Screen.width - edgeMargin - halfWidth),
            Mathf.Clamp(screenPos.y * Screen.height, edgeMargin + halfHeight, Screen.height - edgeMargin - halfHeight)
        );

        indicatorText.text = $"{ArrowFor(viewport, behind)} {best.name} ({distance:0}m)";
        indicatorText.rectTransform.position = screenPx;
        indicatorText.gameObject.SetActive(true);
    }

    private RecycleBinInteractable FindNearestAcceptingBin(ItemType itemType)
    {
        RecycleBinInteractable best = null;
        float bestDistance = float.MaxValue;
        Vector3 origin = playerCamera != null ? playerCamera.transform.position : transform.position;

        foreach (RecycleBinInteractable bin in bins)
        {
            if (bin == null || !bin.AcceptsItem(itemType)) continue;

            float distance = (bin.transform.position - origin).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = bin;
            }
        }

        return best;
    }

    private string ArrowFor(Vector3 viewport, bool behind)
    {
        if (behind) return viewport.x > 0.5f ? "<" : ">";

        if (Mathf.Abs(viewport.x - 0.5f) > Mathf.Abs(viewport.y - 0.5f))
            return viewport.x < 0.5f ? "<" : ">";
        return viewport.y > 0.5f ? "^" : "v";
    }

    #endregion

    #region Utility

    private void EnsureIndicatorText()
    {
        if (indicatorText != null) return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject go = new GameObject("BinIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = 5;
        go.transform.SetParent(canvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(260f, 40f);

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.fontSize = 20f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;

        indicatorText = text;
    }

    private void EnsureNearbyMessageText()
    {
        if (nearbyMessageText != null) return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        GameObject go = new GameObject("BinNearbyMessage", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = 5;
        go.transform.SetParent(canvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -160f);
        rect.sizeDelta = new Vector2(400f, 40f);

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.fontSize = 24f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;

        nearbyMessageText = text;
    }

    #endregion
}
