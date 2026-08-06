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

    [Header("Wind Push")]
    [Tooltip("Max horizontal speed (m/s) the storm wind pushes the player away from a wrong bin.")]
    [SerializeField] private float maxWindPushSpeed = 3f;
    [Tooltip("How quickly the wind push ramps up/down (m/s per second).")]
    [SerializeField] private float windPushRampSpeed = 2.5f;
    [Tooltip("If a correct bin is within this distance, cancel wind push entirely (player is at the right place).")]
    [SerializeField] private float correctBinPushCancelRadius = 5f;

    [Header("Transition")]
    [Tooltip("Cooldown between weather changes to avoid rapid flickering.")]
    [SerializeField] private float stormCooldown = 0.5f;
    [Tooltip("How long the storm persists after a wrong recycle before calming.")]
    [SerializeField] private float wrongRecycleStormDuration = 2f;

    #endregion

    #region Private Fields

    private float cooldownTimer;
    private float wrongRecycleStormTimer;
    private float currentStormIntensity;
    private Vector3 currentWindPush;
    private PickupItem heldItem;
    private PlayerMovement playerMovement;

    #endregion

    #region Public Properties

    /// <summary>Current storm strength in [0, 1], driven by wrong-bin proximity.</summary>
    public float StormIntensity => currentStormIntensity;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

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

        // Wrong recycle storm cooldown: keep weather stormy briefly after a wrong recycle.
        if (wrongRecycleStormTimer > 0f)
        {
            wrongRecycleStormTimer -= Time.deltaTime;
            if (wrongRecycleStormTimer <= 0f)
            {
                // Storm cooldown expired — calm to rainy if no item held.
                if (heldItem == null)
                {
                    currentStormIntensity = 0f;
                    ApplyStormIntensity(0f);
                    weatherState.SetSunny();
                    weatherEffects?.SetWeather(WeatherState.State.Sunny);
                    windEffect?.SetWeatherState(WeatherState.State.Sunny);
                }
            }
            RampWindPush(Vector3.zero);
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

            RampWindPush(Vector3.zero);
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
        Vector3 nearestWrongBinPosition = player.position;
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
                {
                    nearestWrongBinDistance = dist;
                    nearestWrongBinPosition = col.transform.position;
                }

                foundAnyBin = true;
            }
        }

        // No wrong bins around: mild rain while carrying an item.
        if (!foundAnyBin)
        {
            EnsureState(WeatherState.State.Rainy);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
            return;
        }

        // If the player is close to a correct bin, cancel all push — they're at the right place.
        if (nearestCorrectBinDistance <= correctBinPushCancelRadius)
        {
            EnsureState(WeatherState.State.Rainy);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
            return;
        }

        // Storm strength scales with how close the nearest wrong bin is.
        if (nearestWrongBinDistance <= stormActivationRadius)
        {
            float normalizedDistance = 1f - (nearestWrongBinDistance / stormActivationRadius);
            currentStormIntensity = Mathf.Clamp01(normalizedDistance);
            EnsureState(WeatherState.State.Stormy);
            ApplyStormIntensity(currentStormIntensity);

            // Gradual wind push away from the wrong bin, scaled by storm strength.
            Vector3 away = player.position - nearestWrongBinPosition;
            away.y = 0f;
            if (away.sqrMagnitude > 0.01f)
                RampWindPush(away.normalized * (maxWindPushSpeed * currentStormIntensity));
            else
                RampWindPush(Vector3.zero);
        }
        else
        {
            currentStormIntensity = 0f;
            EnsureState(WeatherState.State.Rainy);
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
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

    /// <summary>
    /// Smoothly ramps the current wind push toward the target so the player is
    /// nudged gradually by the storm instead of being shoved by a collider.
    /// </summary>
    private void RampWindPush(Vector3 target)
    {
        currentWindPush = Vector3.MoveTowards(currentWindPush, target, windPushRampSpeed * Time.deltaTime);
        if (playerMovement != null)
            playerMovement.SetWindPush(currentWindPush);
    }

    #endregion

    #region Event Handlers

    private void OnPickedUp(PickupItem item)
    {
        heldItem = item;
        EnsureState(WeatherState.State.Rainy);
        ApplyStormIntensity(0f);
        RampWindPush(Vector3.zero);
    }

    private void OnDropped()
    {
        heldItem = null;
        currentStormIntensity = 0f;
        ApplyStormIntensity(0f);
        weatherState.SetSunny();
        weatherEffects?.SetWeather(WeatherState.State.Sunny);
        windEffect?.SetWeatherState(WeatherState.State.Sunny);
        RampWindPush(Vector3.zero);
    }

    private void OnItemProcessed(bool isCorrect)
    {
        heldItem = null;
        currentStormIntensity = 0f;
        ApplyStormIntensity(0f);
        RampWindPush(Vector3.zero);

        if (isCorrect)
        {
            weatherState.SetSunny();
            weatherEffects?.SetWeather(WeatherState.State.Sunny);
            windEffect?.SetWeatherState(WeatherState.State.Sunny);
        }
        else
        {
            // Wrong recycle: keep storm active briefly as environmental feedback.
            wrongRecycleStormTimer = wrongRecycleStormDuration;
        }
    }

    #endregion
}
