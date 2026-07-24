using UnityEngine;

public class WeatherFeedbackSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private WeatherEffects weatherEffects;
    [SerializeField] private WindEffect windEffect;
    [SerializeField] private WeatherMovementEffect weatherMovementEffect;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Transform player;

    [Header("Bin Detection")]
    [SerializeField] private float binDetectionRadius = 15f;
    [SerializeField] private float stormActivationRadius = 8f;
    [SerializeField] private LayerMask binLayer;

    [Header("Transition")]
    [SerializeField] private float stormCooldown = 0.5f;

    private float cooldownTimer;
    private float currentStormIntensity;
    private PickupItem heldItem;

    public float StormIntensity => currentStormIntensity;

    private void OnEnable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.OnObjectPickedUp += OnPickedUp;
            playerInteraction.OnObjectDropped += OnDropped;
        }

        foreach (var bin in FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None))
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

        foreach (var bin in FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None))
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

    private void EvaluateBinProximity()
    {
        if (player == null || heldItem == null) return;

        float nearestWrongBinDistance = float.MaxValue;
        float nearestCorrectBinDistance = float.MaxValue;
        bool foundAnyBin = false;

        Collider[] nearby = Physics.OverlapSphere(player.position, binDetectionRadius, binLayer);

        foreach (var col in nearby)
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

        if (!foundAnyBin)
        {
            EnsureState(WeatherState.State.Rainy);
            currentStormIntensity = 0f;
            ApplyStormIntensity(0f);
            return;
        }

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
}
