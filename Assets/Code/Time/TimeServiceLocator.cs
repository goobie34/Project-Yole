using System;
using UnityEngine;

public class TimeServiceLocator : ServiceLocator<ITimeService>
{
  
}

public interface ITimeService
{
    public DateTime GetCurrentGameTime();
    public float GetCurrentTimeFloatUnscaled();


    public float GetCurrentGameDayFraction();

    public float GetCurrentGameDayNightFraction();


    public Coroutine WarpGameTimeTo(DateTime targetTime, float realTimeDuration);

    public void SetGameTime(DateTime targetTime);

}
