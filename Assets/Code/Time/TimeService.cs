using System;
using UnityEngine;

public class TimeService : MonoBehaviour, ITimeService
{


    public DateTime CurrentTime;

    public float _24hCycleDurationSeconds = 60;

    public float _timeRate = 1;

    public static readonly int SecondsIn24h = 86400;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeServiceLocator.Instance.Register(this);

    }

    public void Update()
    {
        SetGameTime(CurrentTime.AddSeconds(Time.deltaTime * SecondsIn24h / _24hCycleDurationSeconds * _timeRate));

        
    }

    public void FixedUpdate()
    {
        Debug.Log("TimeTest --- ");
        Debug.Log($"test ({GetCurrentTimeFloatUnscaled()})");
        Debug.Log($"target ({Time.time})");
        
    }

    public DateTime GetCurrentGameTime()
    {
        return CurrentTime;
    }
    public float GetCurrentTimeFloatUnscaled()
    {
        return (CurrentTime.Second + CurrentTime.Millisecond / 1000f) / (SecondsIn24h / _24hCycleDurationSeconds);
    }


    public float GetCurrentGameDayFraction()
    {
        throw new NotImplementedException();
    }

    public float GetCurrentGameDayNightFraction()
    {
        throw new NotImplementedException();
    }


    public Coroutine WarpGameTimeTo(DateTime targetTime, float realTimeDuration)
    {
        throw new NotImplementedException();
    }

    public void SetGameTime(DateTime targetTime)
    {
        throw new NotImplementedException();
    }

}
