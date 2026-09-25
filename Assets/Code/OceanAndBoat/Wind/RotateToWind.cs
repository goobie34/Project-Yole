using UnityEngine;

public class RotateToWind : MonoBehaviour
{
    public Transform childTarget;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!WindServiceLocator.Instance.TryGet(out var windService)) { return; }


        windService.EvaluateWind(transform.position, Time.time, out var windDirection, out var mag);
        var localWind = -transform.InverseTransformDirection(windDirection);
        localWind.y = 0;
        childTarget.localRotation = Quaternion.FromToRotation(Vector3.forward, localWind);
    }
}
