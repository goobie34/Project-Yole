using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public class SailingForce : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private SailingController _controller;

    private Vector3 windDirection = Vector3.forward;
    private Vector3 windDirectionWorld = Vector3.forward;
    public float windMagnitude = 0;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_controller == null)
            _controller = GetComponent<SailingController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetWind();


        var direction = _controller.sailRotator.TransformDirection(1, 0, 0);

        var sailStrength = _controller.appliedAcceleration;

        //float currentDirectionalSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        //var invMaxSpeedFrac = 1 - math.smoothstep(0, maxSpeed, currentDirectionalSpeed);

        //if (currentDirectionalSpeed > maxSpeed)
        //    return;

        //currentSailAccumulation += (Mathf.Abs(sailStrength) - 0.5f) * 2;

        //currentSailAccumulation = Mathf.Clamp(currentSailAccumulation, 0, maxSpeed);

        //accumFrac = math.smoothstep(0, maxSpeed, currentSailAccumulation);

        //rb.AddForce(direction * sailStrength * windMagnitude * Mathf.Lerp(scale,1, accumFrac) * Time.fixedDeltaTime * 60, ForceMode.Acceleration);


        var windForce = (windDirectionWorld * windMagnitude - rb.linearVelocity).magnitude;

        var force = direction * windForce * sailStrength;

        Debug.DrawRay(_controller.sailTipPos.position, force);

        rb.AddForce(direction * windForce * sailStrength * Time.fixedDeltaTime * 60, ForceMode.Acceleration);


    }

    private void GetWind()
    {
        if (!WindServiceLocator.Instance.TryGet(out var windService)) { return; }
        windService.EvaluateWind(transform.position, Time.time, out windDirectionWorld, out windMagnitude);
        windDirection = transform.InverseTransformDirection(windDirectionWorld);
    }
}
