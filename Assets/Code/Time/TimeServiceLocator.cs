using System;
using UnityEngine;

public class TimeServiceLocator : ServiceLocator<ITimeService>
{

}

public interface ITimeService
{
    public TimeSpan GetCurrentGameTime();
    public double GetCurrentTimeFloatUnscaled();


    public double GetCurrentGameDayFraction();


    public float GetCurrentGameDayNightFraction();


    public Coroutine WarpGameTimeTo(TimeSpan targetTime, float realTimeDuration);
    public void SetGameTime(TimeSpan targetTime);

    public TimeSpan GetTime_NextMorning(TimeSpan time);

    

}
