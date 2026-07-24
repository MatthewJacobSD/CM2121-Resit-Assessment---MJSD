using UnityEngine;

public class WeatherTimer : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float[] timePerState = new float[5] { 30f, 25f, 20f, 35f, 15f };

    private float timer;
    private int currentIndex;

    public event System.Action OnTimerExpired;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            CycleTimer();
            OnTimerExpired?.Invoke();
        }
    }

    private void CycleTimer()
    {
        currentIndex = (currentIndex + 1) % timePerState.Length;
        timer = timePerState[currentIndex];
    }

    public void ResetTimer()
    {
        currentIndex = 0;
        timer = timePerState[0];
    }
}