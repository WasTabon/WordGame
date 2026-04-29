using System;
using UnityEngine;

public class EscapeTimer : MonoBehaviour
{
    public float StartTime { get; private set; }
    public float TimeLeft { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTick;
    public event Action OnTimeout;

    private bool timeoutFired;

    public void Begin(float seconds)
    {
        StartTime = seconds;
        TimeLeft = seconds;
        IsRunning = true;
        timeoutFired = false;
        if (OnTick != null) OnTick.Invoke(TimeLeft);
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void AddTime(float seconds)
    {
        TimeLeft += seconds;
        if (OnTick != null) OnTick.Invoke(TimeLeft);
    }

    private void Update()
    {
        if (!IsRunning) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            IsRunning = false;
            if (OnTick != null) OnTick.Invoke(0f);
            if (!timeoutFired && OnTimeout != null)
            {
                timeoutFired = true;
                OnTimeout.Invoke();
            }
            return;
        }
        if (OnTick != null) OnTick.Invoke(TimeLeft);
    }
}
