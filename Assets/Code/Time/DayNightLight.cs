using UnityEngine;

public class DayNightLight : MonoBehaviour
{
    public Transform sunTransform;

    public float DayNightFrac = 0;

    public float testAngle = 0;

    public float rotationsPerDay = 1;

   

    // Update is called once per frame
    void Update()
    {
        if(TimeServiceLocator.Instance == null)
            return;

        if(!TimeServiceLocator.Instance.TryGet(out var service))
            return;

        

        DayNightFrac = (float)service.GetCurrentGameTime().TotalDays * rotationsPerDay;

        RotateSun(DayNightFrac);
    }

    private void RotateSun(float dayFrac)
    {
        if (sunTransform == null)
            return;

        Vector3 axis = transform.forward;

        var angle = (dayFrac * 360) % 360f;

        sunTransform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

}
