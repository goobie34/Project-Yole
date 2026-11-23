using System;
using System.Collections;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class TimeService : MonoBehaviour, ITimeService
{
    public int startHour;
    public int startMinute;

    public TimeSpan CurrentTime;

    public float _24hCycleDurationSeconds = 60;

    public float _timeRate = 1;

    public static readonly int SecondsIn24h = 86400;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeServiceLocator.Instance.Register(this);

        var cStartHour = math.clamp(startHour, 0, 23);
        var cStartMinute = math.clamp(startMinute, 0, 60);

        SetGameTime(new TimeSpan(cStartHour, cStartMinute, 0));

    }

  

    public void FixedUpdate()
    {
        if (_24hCycleDurationSeconds < 0.001f)
            return;

        

        SetGameTime(CurrentTime.Add(TimeSpan.FromSeconds(Time.fixedDeltaTime * (SecondsIn24h / _24hCycleDurationSeconds) * _timeRate)));
        
    }

    public TimeSpan GetCurrentGameTime()
    {
        return CurrentTime;
    }
    public double GetCurrentTimeFloatUnscaled()
    {
        return CurrentTime.TotalSeconds / (SecondsIn24h / _24hCycleDurationSeconds);
    }


    public double GetCurrentGameDayFraction()
    {
        return CurrentTime.TotalDays - (double)CurrentTime.Days;
    }


    public float GetCurrentGameDayNightFraction()
    {
        throw new NotImplementedException();
    }


    public Coroutine WarpGameTimeTo(TimeSpan targetTime, float realTimeDuration)
    {
        return StartCoroutine(LerpGameTimeCorutine(realTimeDuration, CurrentTime, targetTime));
    }

    private IEnumerator LerpGameTimeCorutine(float duration, TimeSpan start, TimeSpan end)
    {
        _timeRate = 0;

        float timer = 0;

        float lerp = 0;


        while (lerp < 1)
        {
            var lerpTime = LerpTimespan(start, end, SmoothCurve(lerp));

            SetGameTime(lerpTime);

            timer += Time.deltaTime;

            lerp = Mathf.Clamp01(timer / duration);

            yield return null;
        }

        SetGameTime(end);

        _timeRate = 1;
    }

    private TimeSpan LerpTimespan(TimeSpan a, TimeSpan b, float t)
    {
        return b * t + a * (1 - t);
    }

    private float SmoothCurve(float t)
    {
        return 1 - ((Mathf.Cos(t * Mathf.PI) + 1) * 0.5f);
    }

    public void SetGameTime(TimeSpan targetTime)
    {
        CurrentTime = targetTime;
    }

    public TimeSpan GetTime_NextMorning(TimeSpan time)
    {
        var days = math.ceil(time.TotalDays);

        return TimeSpan.FromDays(days);
    }


}
