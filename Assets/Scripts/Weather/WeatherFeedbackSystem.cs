using UnityEngine;

/// <summary>
/// Reacts to gameplay (carrying items near the wrong bin) by shifting the
/// weather to rain or storm, simulating environmental consequences.
/// </summary>
public class WeatherFeedbackSystem : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private WeatherEffects weatherEffects;
    [SerializeField] private WindEffect windEffect;
    [SerializeField] private WeatherMovementEffect weatherMovementEffect;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Transform player;

    [Header("Bin Detection")]
    [Tooltip("Radius in which bins are considered when carrying an item.")]
    [SerializeField] private float binDetectionRadius = 15f;
    [Tooltip("Distance to a wrong bin that triggers a full storm.")]
    [SerializeField] private float stormActivationRadius = 8f;
    [SerializeField] private LayerMask binLayer;

    [Header("Transition")]
    [Tooltip("Cooldown between weather changes to avoid rapid flickering.")]
    [SerializeField] private float stormCooldown = 0.5f;

    #endregion

    #region Private Fields

    private float cooldownTimer;
    private float currentStormIntensity;
    private PickupItem heldItem;

    #endregion

    #region Public Properties

    /// <summary>Current storm strength in [0, 1], driven by wrong-bin proximity.</summary>
    public float StormIntensity => currentStormIntensity;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.OnObjectPickedUp += OnPickedUp;
            playerInteraction.OnObjectDropped += OnDropped;
        }

        foreach (RecycleBinInteractable bin in FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None))
        {
            bin.OnItemProcessed += OnItemProcessed;
        }
    }

    private void OnDisable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.OnObjectPickedUp -= OnPickedUp;
            playerInteraction.OnObjectDropped -= OnDropped;
        }

        foreach (RecycleBinInteractable bin in FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None))
        {
            bin.OnItemProcessed -= OnItemProcessed;
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        // No item held (or not playing): calm the storm back to zero.
        if (heldItem == null || !GameManager.Instance.IsPlaying)
        {
            if (currentStormIntensity > 0f)
            {
                currentStormIntensity = 0f;
                weatherEffects?.SetStormIntensity(0f);
                windEffect?.SetStormIntensity(0f);
                weatherMovementEffect?.SetStormIntensity(0f);
            }
            return;
        }

        EvaluateBinProximity();
    }

    #endregion

    #region Storm Proximity Logic

    private void EvaluateBinProximity()
    {
        if (player == null || heldItem == null) return;

        float nearestWrongBinDistance = float.MaxValue;
        float nearestCorrectBinDistance = float.MaxValue;
        bool foundAnyBin = false;

        Collider[] nearby = Physics.OverlapSphere(player.position, binDetectionRadius, binLayer);

        foreach (Collider col in nearby)
        {
            RecycleBinInteractable bin = col.GetComponentInParent<RecycleBinInteractable>();
            if (bin == null) continue;

            float dist = Vector3.Distance(player.position, col.transform.position);
            bool accepts = bin.AcceptsItem(heldItem.ItemType);

            if (accepts)
            {
                if (dist < nearestCorrectBinDistance)
                    nearestCorrectBinDistance = dist;
            }
            else
            {
                if (dist < nearestWrongBinDistance)
                    nearestWrongBinDistance = dist;

                foundAnyBin = true;
            }
        }

        // No wrong bins around: mild rain while carrying an item.
        if (!foundAnyBin)
        {
            EnsureState(WeatherState.State.Rainy);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            return;
        }

        // Storm strength scales with how close the nearest wrong bin is.
        if (nearestWrongBinDistance <= stormActivationRadius)
        {
            float normalizedDistance = 1f - (nearestWrongBinDistance / stormActivationRadius);
            currentStormIntensity = Mathf.Clamp01(normalizedDistance);
            EnsureState(WeatherState.State.Stormy);
            ApplyStormIntensity(currentStormIntensity);
        }
        else
        {
            currentStormIntensity = 0f;
            EnsureState(WeatherState.State.Rainy);
            ApplyStormIntensity(0f);
        }
    }

    private void EnsureState(WeatherState.State target)
    {
        if (weatherState.GetCurrentState() != target)
        {
            weatherState.SetWeather(target);
            weatherEffects?.SetWeather(target);
            windEffect?.SetWeatherState(target);
            cooldownTimer = stormCooldown;
        }
    }

    private void ApplyStormIntensity(float intensity)
    {
        weatherEffects?.SetStormIntensity(intensity);
        windEffect?.SetStormIntensity(intensity);
        weatherMovementEffect?.SetStormIntensity(intensity);
    }

    #endregion

    #region Event Handlers

    private void OnPickedUp(PickupItem item)
    {
        heldItem = item;
        EnsureState(WeatherState.State.Rainy);
        ApplyStormIntensity(0f);
    }

    private void OnDropped()
    {
        heldItem = null;
        currentStormIntensity = 0f;
        ApplyStormIntensity(0f);
        weatherState.SetSunny();
        weatherEffects?.SetWeather(WeatherState.State.Sunny);
        windEffect?.SetWeatherState(WeatherState.State.Sunny);
    }

    private void OnItemProcessed(bool isCorrect)
    {
        heldItem = null;
        currentStormIntensity = 0f;
        ApplyStormIntensity(0f);

        if (isCorrect)
        {
            weatherState.SetSunny();
            weatherEffects?.SetWeather(WeatherState.State.Sunny);
            windEffect?.SetWeatherState(WeatherState.State.Sunny);
        }
    }

    #endregion
}
