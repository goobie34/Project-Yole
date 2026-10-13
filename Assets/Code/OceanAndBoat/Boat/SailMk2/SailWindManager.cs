using UnityEngine;

public class SailWindManager : MonoBehaviour
{
    [SerializeField] private Vector3 windDirection;
    [SerializeField] private float windMagnitude;


    private Vector3 windVector_world;
    private Vector3 windVector_local;
    private float windAngleLocal;

    public float GetLocalWindAngle() => windAngleLocal;
    public Vector3 GetCurrentWind() => windVector_world;
    public Vector3 GetLocalWindDirection() => windVector_world;

    public void FixedUpdate()
    {
        if(WindServiceLocator.Instance == null)
            return;

        if (!WindServiceLocator.Instance.TryGet(out var windService))
            return;

        

        windService.EvaluateWind(transform.position, Time.time, out windDirection, out windMagnitude);


        windVector_world = windDirection * windMagnitude;

        windVector_local = transform.InverseTransformDirection(GetCurrentWind());

        windAngleLocal = Vector3.SignedAngle(Vector3.forward, -windVector_local.normalized, Vector3.up);

    }

    
   
}
