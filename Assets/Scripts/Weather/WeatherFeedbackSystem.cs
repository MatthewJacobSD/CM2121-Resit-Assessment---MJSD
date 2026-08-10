using UnityEngine;

/// <summary>
/// Drives weather transitions based on gameplay:
/// - Sunny when no item held
/// - Stays sunny when picking up an item
/// - Light rain when approaching a wrong bin
/// - Heavy rain when closer to a wrong bin
/// - Storm on wrong recycle (with wind push)
/// - Progressive calming when approaching a correct bin
/// - Sunny on correct recycle
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
    [Tooltip("Distance at which light rain begins.")]
    [SerializeField] private float rainStartRadius = 15f;
    [Tooltip("Distance at which heavy rain begins.")]
    [SerializeField] private float heavyRainRadius = 10f;
    [Tooltip("Distance at which storm begins.")]
    [SerializeField] private float stormRadius = 6f;
    [SerializeField] private LayerMask binLayer;

    [Header("Wind Push")]
    [Tooltip("Max horizontal speed (m/s) the storm wind pushes the player away from a wrong bin.")]
    [SerializeField] private float maxWindPushSpeed = 3f;
    [Tooltip("How quickly the wind push ramps up/down (m/s per second).")]
    [SerializeField] private float windPushRampSpeed = 2.5f;
    [Tooltip("If a correct bin is within this distance, cancel wind push entirely.")]
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
    private Collider[] binOverlapBuffer = new Collider[16];

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

            // If player dropped/lost the item during cooldown, calm immediately.
            if (heldItem == null)
            {
                CalmToSunny();
                return;
            }

            // Storm cooldown expired — transition to heavy rain, then calm.
            if (wrongRecycleStormTimer <= 0f)
            {
                EnsureState(WeatherState.State.HeavyRain);
                currentStormIntensity = 0f;
                ApplyStormIntensity(0f);
            }
            RampWindPush(Vector3.zero);
            return;
        }

        // No item held (or not playing): calm back to sunny.
        if (heldItem == null || !GameManager.Instance.IsPlaying)
        {
            CalmToSunny();
            return;
        }

        EvaluateBinProximity();
    }

    #endregion

    #region Weather Proximity Logic

    private void EvaluateBinProximity()
    {
        if (player == null || heldItem == null) return;

        float nearestWrongBinDistance = float.MaxValue;
        float nearestCorrectBinDistance = float.MaxValue;
        Vector3 nearestWrongBinPosition = player.position;
        bool foundAnyBin = false;

        int binCount = Physics.OverlapSphereNonAlloc(player.position, binDetectionRadius, binOverlapBuffer, binLayer);

        for (int i = 0; i < binCount; i++)
        {
            Collider col = binOverlapBuffer[i];
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

        // No wrong bins around: weather stays sunny while carrying an item.
        if (!foundAnyBin)
        {
            CalmToSunny();
            return;
        }

        // If the player is close to a correct bin, calm down — they're at the right place.
        if (nearestCorrectBinDistance <= correctBinPushCancelRadius)
        {
            EnsureState(WeatherState.State.Sunny);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
            return;
        }

        // Progressive weather based on distance to nearest wrong bin.
        if (nearestWrongBinDistance <= stormRadius)
        {
            // Close to wrong bin: storm.
            float normalizedDistance = 1f - (nearestWrongBinDistance / stormRadius);
            currentStormIntensity = Mathf.Clamp01(normalizedDistance);
            EnsureState(WeatherState.State.Stormy);
            ApplyStormIntensity(currentStormIntensity);

            // Wind push away from the wrong bin, scaled by storm strength.
            Vector3 away = player.position - nearestWrongBinPosition;
            away.y = 0f;
            if (away.sqrMagnitude > 0.01f)
                RampWindPush(away.normalized * (maxWindPushSpeed * currentStormIntensity));
            else
                RampWindPush(Vector3.zero);
        }
        else if (nearestWrongBinDistance <= heavyRainRadius)
        {
            // Medium distance: heavy rain.
            EnsureState(WeatherState.State.HeavyRain);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
        }
        else if (nearestWrongBinDistance <= rainStartRadius)
        {
            // Far from wrong bin: light rain.
            EnsureState(WeatherState.State.Rainy);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            RampWindPush(Vector3.zero);
        }
        else
        {
            // Too far from any wrong bin: sunny.
            CalmToSunny();
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

    private void CalmToSunny()
    {
        if (currentStormIntensity > 0f || weatherState.GetCurrentState() != WeatherState.State.Sunny)
        {
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            EnsureState(WeatherState.State.Sunny);
        }
        RampWindPush(Vector3.zero);
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
        // Weather stays sunny — rain only starts when approaching a wrong bin.
        currentStormIntensity = 0f;
        ApplyStormIntensity(0f);
        RampWindPush(Vector3.zero);
    }

    private void OnDropped()
    {
        heldItem = null;
        CalmToSunny();
    }

    private void OnItemProcessed(bool isCorrect)
    {
        heldItem = null;

        if (isCorrect)
        {
            // Correct recycle: sunny immediately.
            CalmToSunny();
        }
        else
        {
            // Wrong recycle: storm feedback, then calm.
            currentStormIntensity = 1f;
            ApplyStormIntensity(1f);
            EnsureState(WeatherState.State.Stormy);
            wrongRecycleStormTimer = wrongRecycleStormDuration;
        }
    }

    #endregion
}
