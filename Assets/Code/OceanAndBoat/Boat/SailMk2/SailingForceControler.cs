using UnityEngine;

public class SailingForceControler : MonoBehaviour
{
    [Header("Rope Properties")]

    public float hitchPointLength = 1f;

    public float mainsailRopeLength;

    [Header("Angular properties")]

    public float sailAngularSpeedMult = 3;

    [Header("Force properties")]

    public float effectiveAnglePow = 3f;


    [Header("Dependencies")]
    public SailWindManager windManager;
    public SailPhysics sailSimulator;
    public Rigidbody rb;

    [Header("Data")]
    public float maxAngle;
    public float sailWindDot;
    public float pushAmount;
    public float appliedAcceleration;
    

    public Vector3 localWindDirection;
    public float localWindMagnitude;
    public float localWindAngle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(windManager == null)
            windManager = GetComponent<SailWindManager>();

        if(sailSimulator == null)
            sailSimulator = GetComponent<SailPhysics>();

        if(rb == null) rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateSailingForceControler(Time.fixedDeltaTime * 50);

        

    }

    public void UpdateSailingForceControler(float normalizedDeltaTime)
    {
        UpdateAngles();

        GetWind();

        if (sailSimulator == null)
            return;

        ApplyAngularWindForce(normalizedDeltaTime);

        ClampSail(ref sailSimulator.sailAngle, ref sailSimulator.sailAngularVelocity, ref sailSimulator.sailAngularAcceleration);




        ApplySailForceToBoat(normalizedDeltaTime);

    }

    private void GetWind()
    {
        if (windManager == null)
            return;

        localWindDirection = windManager.GetLocalWindDirection();

        Vector3 linearVelocity = rb != null ? rb.linearVelocity : Vector3.zero;

        float windDifference = Mathf.Max(localWindDirection.magnitude - Vector3.Dot(transform.forward, linearVelocity), 0);  


        localWindMagnitude = windDifference;

        localWindDirection.Normalize();

        localWindAngle = windManager.GetLocalWindAngle();

    }

    private void UpdateAngles()
    {
        maxAngle = GetMaxAngleFromTrig(hitchPointLength, hitchPointLength, mainsailRopeLength) * Mathf.Rad2Deg;


    }
    
  

    private void ClampSail(ref float sailAngle, ref float sailAngularVelocity, ref float angularAcceleration)
    {
        if (sailAngle <= -maxAngle)
        {
            sailAngle = -maxAngle;

            pushAmount = sailWindDot;

            sailAngularVelocity = Mathf.Clamp(sailAngularVelocity, 0, maxAngle);

            angularAcceleration = Mathf.Clamp(angularAcceleration, 0, maxAngle);

            return;
        }

        if (sailAngle >= maxAngle)
        {
            sailAngle = maxAngle;


            pushAmount = sailWindDot;

            sailAngularVelocity = Mathf.Clamp(sailAngularVelocity, -maxAngle, 0);

            angularAcceleration = Mathf.Clamp(angularAcceleration, -maxAngle, 0);

            return;
        }

        pushAmount = 0;


    }

    private void ApplyAngularWindForce(float deltaTime)
    {


        sailWindDot = AngleDot(localWindAngle * Mathf.Deg2Rad, (sailSimulator.sailAngle + 90) * Mathf.Deg2Rad);


        float angularAcc = Mathf.Clamp(sailWindDot * sailAngularSpeedMult * localWindMagnitude, -maxAngle - sailSimulator.sailAngle, maxAngle - sailSimulator.sailAngle);

        sailSimulator.ApplyAngularAccelerationToSail(angularAcc * deltaTime);
    }

    private void ApplySailForceToBoat(float deltaTime)
    {
        appliedAcceleration = Mathf.Pow(Mathf.Abs(pushAmount), effectiveAnglePow) * (pushAmount < 0 ? -1 : 1);


        Vector3 sailDirection = (sailSimulator.sailRotationWorld * Vector3.left);

        Debug.DrawRay(transform.position, sailDirection);

        var force = sailDirection * appliedAcceleration * localWindMagnitude;
        rb.AddForce(force * (deltaTime), ForceMode.Acceleration);
    }


    #region MathHelpers
    private static float GetMaxAngleFromTrig(float mastL, float boatL, float ropeL)
    {
        return Mathf.Acos((mastL * mastL + boatL * boatL - ropeL * ropeL) / (2 * mastL * boatL));
    }

    private static float AngleDot(float a, float b)
    {
        return Mathf.Cos(a) * Mathf.Cos(b) + Mathf.Sin(a) * Mathf.Sin(b);
    }
    #endregion
}
