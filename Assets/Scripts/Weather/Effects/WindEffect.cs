using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField] private WindZone windZone;
    [SerializeField] private ParticleSystem windParticles;

    [SerializeField] private float maxWindSpeed = 25f;

    private void Awake()
    {
        if (windZone == null)
            windZone = GetComponent<WindZone>();
    }

    public void SetActive(bool active)
    {
        float speed = active ? 12f : 0f;
        SetWindSpeed(speed);
    }

    public void SetWindSpeed(float speed)
    {
        float clamped = Mathf.Clamp(speed, 0f, maxWindSpeed);

        if (windZone != null)
            windZone.windMain = clamped;

        if (windParticles != null)
        {
            var em = windParticles.emission;
            em.rateOverTime = clamped * 4f;

            var main = windParticles.main;
            main.startSpeed = clamped * 0.5f;
        }
    }
}