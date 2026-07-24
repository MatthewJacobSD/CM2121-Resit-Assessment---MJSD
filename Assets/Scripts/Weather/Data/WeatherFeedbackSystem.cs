using UnityEngine;

public class WeatherFeedbackSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeatherState weatherState;
    [SerializeField] private WeatherEffects weatherEffects;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Settings")]
    [SerializeField] private float stormProximityDistance = 8f;
    [SerializeField] private float stormIntensityMultiplier = 1.5f;

    private PickupItem heldItem;
    private RecycleBinInteractable nearestWrongBin;

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        heldItem = playerInteraction.CurrentHeldObject;

        if (heldItem != null)
        {
            CheckProximityToWrongBin();
        }
        else
        {
            // Reset to Sunny when not holding anything wrong
            if (weatherState.GetCurrentState() != WeatherState.State.Sunny)
                weatherState.SetToSunny(); // You'll need to add this method
        }
    }

    private void CheckProximityToWrongBin()
    {
        // Find nearest bin
        nearestWrongBin = FindNearestWrongBin();

        if (nearestWrongBin != null)
        {
            float distance = Vector3.Distance(transform.position, nearestWrongBin.transform.position);

            if (distance < stormProximityDistance)
            {
                float intensity = Mathf.Lerp(0.3f, 1f, 1 - (distance / stormProximityDistance));
                weatherEffects.SetStormIntensity(intensity); // Custom method
                weatherState.SetToStormy();
            }
        }
    }

    private RecycleBinInteractable FindNearestWrongBin()
    {
        // Simple implementation - you can optimize with Physics.OverlapSphere later
        var bins = FindObjectsByType<RecycleBinInteractable>(FindObjectsSortMode.None);
        RecycleBinInteractable closest = null;
        float minDist = float.MaxValue;

        foreach (var bin in bins)
        {
            if (IsWrongBinForItem(bin, heldItem))
            {
                float dist = Vector3.Distance(transform.position, bin.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = bin;
                }
            }
        }
        return closest;
    }

    private bool IsWrongBinForItem(RecycleBinInteractable bin, PickupItem item)
    {
        // You can expand this logic based on your BinType
        return bin.binType != GetCorrectBinForItem(item.ItemType);
    }

    private BinType GetCorrectBinForItem(ItemType type)
    {
        return type switch
        {
            ItemType.Plant => BinType.NatureRecycling,
            ItemType.Bottle => BinType.PlasticRecycling,
            ItemType.Toy => BinType.GeneralWaste,
            _ => BinType.NatureRecycling
        };
    }
}