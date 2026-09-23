using UnityEngine;

public class SimpleDayNightCylce : MonoBehaviour
{
    public float cycleDuration = 100f;

    public Transform sunTransform;

    public float DayNightFrac = 0;

    public float testAngle = 0;
   

    public float GetTimeDayNightTime(float worldTime)
    {
        return (worldTime % cycleDuration) / cycleDuration;
    }

    // Update is called once per frame
    void Update()
    {
        DayNightFrac = GetTimeDayNightTime(Time.time);

        RotateSun(DayNightFrac);
    }

    private void RotateSun(float dayFrac)
    {
        if (sunTransform == null)
            return;

        Vector3 axis = transform.forward;

        var angle = dayFrac * 360;

        sunTransform.localRotation = Quaternion.Euler(angle, 0,0 );
    }

    
}
