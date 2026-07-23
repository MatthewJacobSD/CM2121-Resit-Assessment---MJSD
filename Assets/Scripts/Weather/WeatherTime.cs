using UnityEngine;

public class WeatherTimer : MonoBehaviour
{
    [Header("Duration")]
    [SerializeField] float timer;

    private int currentStateIndex;
    public float[] timesForWeatherStates = new float[7];

    public delegate void TimerExpiredHandler();
    public event TimerExpiredHandler OnTimerExpired;
    
    private void Start()
    {
        currentStateIndex = 0;
        timer = timesForWeatherStates[currentStateIndex];
    }

    private void Update()
    {
        timer = -Time.deltaTime;

        if (timer <= 0)
        {
            CycleTimer();
            OnTimerExpired?.Invoke();
        }
    }

    public void CycleTimer()
    {
        SetTimer(currentStateIndex + 1);
    }

    public void SetTimer(int nextStateIndex)
    {
        currentStateIndex = nextStateIndex % timesForWeatherStates.Length;
        timer = timesForWeatherStates[currentStateIndex];
    }

    public void SetTimer(float time)
    {
        timer = time;
    }

    public float GetCurrentTime()
    {
        return timer;
    }
}