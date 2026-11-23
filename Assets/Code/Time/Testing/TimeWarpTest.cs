using System;
using UnityEngine;

public class TimeWarpTest : MonoBehaviour
{

    public bool btn;

    public int days = 1;

    public float duration = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (btn)
        {
            btn = false;

            DoTest();
        }
    }

    public void DoTest()
    {
        if(TimeServiceLocator.Instance == null)
            return;

        if (!TimeServiceLocator.Instance.TryGet(out var service))
            return;

        var targetTime = service.GetCurrentGameTime() + TimeSpan.FromDays(days);

        targetTime = service.GetTime_NextMorning(targetTime);

        service.WarpGameTimeTo(targetTime, duration);

    }
}
