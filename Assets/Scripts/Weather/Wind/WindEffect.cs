using UnityEngine;

public class WindEffect : MonoBehaviour
{
    // Reference to Unity's built-in WindZone component.
    // Controls environmental wind effects such as trees, grass,
    // and other wind-reactive objects.
    [SerializeField] private WindZone windZone;


    // Optional particle system used to visually represent wind.
    // For example: leaves, dust, or moving air particles.
    [SerializeField] private ParticleSystem windParticles;


    // Maximum wind strength allowed.
    // Prevents extreme values from affecting performance or gameplay.
    [SerializeField] private float maxWindSpeed = 25f;


    private void Awake()
    {
        // If no WindZone has been assigned in the Inspector,
        // automatically try to find one on this GameObject.
        if (windZone == null)
            windZone = GetComponent<WindZone>();
    }


    // Updates the wind intensity based on the current weather state.
    // Called by WeatherEffects during weather transitions.
    public void SetWindSpeed(float speed)
    {
        // Restrict the wind strength between 0 and the maximum value.
        float clamped = Mathf.Clamp(speed, 0f, maxWindSpeed);


        // Apply the wind strength to Unity's WindZone system.
        // Affects environment objects that support wind movement.
        if (windZone != null)
            windZone.windMain = clamped;


        // Update visual wind particles if available.
        if (windParticles != null)
        {
            // Controls how many wind particles appear over time.
            var emission = windParticles.emission;
            emission.rateOverTime = clamped * 4f;


            // Controls how fast the wind particles move.
            var main = windParticles.main;
            main.startSpeed = clamped * 0.5f;
        }
    }
}