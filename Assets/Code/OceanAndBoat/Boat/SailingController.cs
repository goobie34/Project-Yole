using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SailingController : MonoBehaviour
{
    
    public float sailAngle;
    public float sailAngularVelocity;

    public float sailWindDot = 0;

    public float windAngle;
    public float maxAngle;

    public float sailRotationAcceleration = 10f;
    public float sailAngularDampening = 0.5f;

    public Transform mastOriginPos;
    public Transform sailTipPos;
    public Transform ropeHitchPos;

    public Transform sailRotator;

    private float mastL;
    private float boatL;
    public float ropeL = 1;

    public float ropeMin = 0.7f;
    public float ropeMax = 4f;



    private Vector3 sailDirection;
    private Vector3 windDirection = Vector3.forward;
    private Vector3 windDirectionWorld = Vector3.forward;
    private float windMagnitude = 0;

    public float appliedAcceleration;
    public float sailAccelerationPow = 1;

    public float inputValue;
    public float ropeInputSpeed = 0.01f;

    private float angularWorldDelta = 0;
    private float worldSailAngle = 0;

    public float sailInertia = 1f;

    private Rigidbody rb;

    public BoatRotationManager rotator;
    public Vector2 windLeanStrength;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rotator == null)
            rotator = GetComponentInChildren<BoatRotationManager>();

        CalculateLengths();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateRope();

        GetWind();
        UpdateAngles();

        ApplyWindToSail();

        sailAngularVelocity += angularWorldDelta * Time.fixedDeltaTime * 60;

        ClampSail();

        UpdatePysics();

        var  newWorldSailAngle = transform.rotation.eulerAngles.y + sailAngle;

        angularWorldDelta = Mathf.DeltaAngle(newWorldSailAngle, worldSailAngle);
        worldSailAngle = newWorldSailAngle;



        sailRotator.localRotation = Quaternion.Euler(0, sailAngle, 0);


        ApplySailForce();

        //var leaningPitch = -Vector3.Dot(transform.forward, force);

        //var leaningRoll = -Vector3.Dot(transform.right, force);
        

        //rotator?.ApplyLocalAngularAcceleration(new Vector2(leaningPitch, leaningRoll) * windLeanStrength);


    }

    private void UpdateRope()
    {
        ropeL += inputValue * ropeInputSpeed * Time.deltaTime;

        ropeL = Mathf.Clamp(ropeL, ropeMin, ropeMax);
    }

    private void ApplyWindToSail()
    {
        sailWindDot = AngleDot(windAngle * Mathf.Deg2Rad, (sailAngle + 90) * Mathf.Deg2Rad);



        sailAngularVelocity += (sailWindDot * sailRotationAcceleration * windMagnitude) / sailInertia;
    }

    private void ApplySailForce()
    {
        float sailAcceleration = Mathf.Pow(Mathf.Abs(appliedAcceleration), sailAccelerationPow) * (appliedAcceleration < 0 ? -1 : 1);

        var force = sailRotator.TransformDirection(1, 0, 0) * sailAcceleration * windMagnitude;
        rb.AddForce(force * Time.fixedDeltaTime * 60, ForceMode.Acceleration);
    }

    private float AngleDot(float a, float b)
    {
        return Mathf.Cos(a) * Mathf.Cos(b) + Mathf.Sin(a) * Mathf.Sin(b) ;
    }

    private void GetWind()
    {
        if (!WindServiceLocator.Instance.TryGet(out var windService)) { return; }
        
        
        windService.EvaluateWind(transform.position, Time.time, out windDirectionWorld, out windMagnitude);

        //Debug.DrawRay(sailTipPos.position, windDirectionWorld * windMagnitude, Color.red);



        float windDifference = Mathf.Max(windMagnitude - Vector3.Dot(transform.forward, rb.linearVelocity),0);  //Mathf.Max(windMagnitude - rb.linearVelocity.magnitude, 0);




        windMagnitude = windDifference;

        //windDirectionWorld = windDirectionWorld * windMagnitude - rb.linearVelocity;

        //windMagnitude = windDirectionWorld.magnitude;

        //windDirectionWorld = windDirectionWorld.normalized;

        windDirection = transform.InverseTransformDirection(windDirectionWorld);

        //Debug.DrawRay(sailTipPos.position, windDirectionWorld * windMagnitude, Color.limeGreen);

    }

    private void UpdateAngles()
    {

        maxAngle = GetMaxAngleFromTrig(boatL, boatL, ropeL) * Mathf.Rad2Deg;

        windAngle = Vector3.SignedAngle(Vector3.forward, -windDirection, Vector3.up) ;

        
    }

    private void UpdatePysics()
    {
        

        sailAngle += sailAngularVelocity * Time.fixedDeltaTime;

        sailAngularVelocity *= sailAngularDampening;

        
    }

    private void ClampSail()
    {
        if(sailAngle <= -maxAngle)
        {
            sailAngle = -maxAngle;

            appliedAcceleration = sailWindDot;

            sailAngularVelocity = Mathf.Max(sailAngularVelocity, 0);

            return;
        }

        if (sailAngle >= maxAngle)
        {
            sailAngle = maxAngle;


            appliedAcceleration = sailWindDot;

            sailAngularVelocity = Mathf.Min(sailAngularVelocity, 0);

            return;
        }

        appliedAcceleration = 0;


        
    }

    private void CalculateLengths()
    {
        var flat = new Vector3(1, 0, 1);

        mastL = Vector3.Distance(
            Vector3.Scale(mastOriginPos.position, flat),
            Vector3.Scale(sailTipPos.position, flat));

        boatL = Vector3.Distance(
            Vector3.Scale(mastOriginPos.position, flat),
            Vector3.Scale(ropeHitchPos.position, flat));
    }

    private float GetMaxAngleFromTrig(float mastL, float boatL, float ropeL)
    {
        return Mathf.Acos((mastL * mastL + boatL * boatL - ropeL * ropeL) / (2 * mastL * boatL));
    }

    private void OnExtendSail(InputValue input)
    {
        inputValue = input.Get<float>();

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, sailDirection);
    }

}
