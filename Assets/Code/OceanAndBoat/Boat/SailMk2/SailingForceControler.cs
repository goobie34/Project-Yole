using UnityEngine;

public class SailingForceControler : MonoBehaviour
{
    [Header("Rope Properties")]

    public float hitchPointLength = 1f;

    public float mainsailRopeLength;

    [Header("Angular properties")]

    public float sailAngularSpeedMult = 3;
    public float ropeClampMaxVelocity = 5f;
    public float ropeClampVelocityMult = 2;

    [Header("Force properties")]

    public float effectiveAnglePow = 3f;

    public float sailMomentumForceMult = 5f;

    public float Epsion = 0.1f;

    [Header("Dependencies")]
    public SailWindManager windManager;
    public SailPhysics sailSimulator;
    public Rigidbody rb;
    public BoatVariableManager boatFlags;
    private float sailForceFlag => boatFlags != null ? boatFlags.Get_SailForce : 1;
    private float sailRopeFreeFlag => boatFlags != null ? boatFlags.Get_FreeMainsailRope : 1;


    [Header("Data")]
    public float maxAngle;
    public float sailWindDot;
    public float pushAmount;
    public float sailMomentumPush;
    public float appliedAcceleration;

    public float appliedForce;
    public float appliedSurge;
    

    public Vector3 localWindDirection;
    public float localWindMagnitude;
    public float windMagnitude;
    public float localWindAngle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(boatFlags == null)
            boatFlags = GetComponentInParent<BoatVariableManager>();

        if (windManager == null)
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

        windMagnitude = localWindDirection.magnitude;

        Vector3 linearVelocity = rb != null ? rb.linearVelocity : Vector3.zero;

        float windDifference = Mathf.Max(localWindDirection.magnitude - Vector3.Dot(transform.forward, linearVelocity), 0);  


        localWindMagnitude = windDifference;

        localWindDirection.Normalize();

        localWindAngle = windManager.GetLocalWindAngle();

    }

    private void UpdateAngles()
    {
        
        float mainsailRopeLength = Mathf.Lerp(0,this.mainsailRopeLength, sailRopeFreeFlag);

        maxAngle = GetMaxAngleFromTrig(hitchPointLength, hitchPointLength, mainsailRopeLength) * Mathf.Rad2Deg;


    }
    
  

    private void ClampSail(ref float mSailAngle, ref float mSailAngularVelocity, ref float mAngularAcceleration)
    {
        float sailAngle = mSailAngle;
        float sailAngularVelocity = mSailAngularVelocity;
        float angularAcceleration = mAngularAcceleration;


        if (mSailAngle <= -maxAngle + Epsion)
        {

            pushAmount = sailWindDot;

            sailMomentumPush = mSailAngularVelocity;




            sailAngularVelocity = Mathf.Min(Mathf.Max(sailAngularVelocity, (-maxAngle - mSailAngle) ), maxAngle - mSailAngle);
            sailAngularVelocity = Mathf.Min(sailAngularVelocity, ropeClampMaxVelocity);

            sailMomentumPush -= sailAngularVelocity;

            angularAcceleration = Mathf.Clamp(angularAcceleration, 0, maxAngle);

            if (mSailAngle > -maxAngle)
                return;


            mSailAngle = sailAngle;
            mSailAngularVelocity = sailAngularVelocity;
            mAngularAcceleration = angularAcceleration;


            return;
        }

        if (mSailAngle >= maxAngle - Epsion)
        {
            //sailAngle = maxAngle;


            pushAmount = sailWindDot;

            sailMomentumPush = mSailAngularVelocity;



            sailAngularVelocity = Mathf.Max(Mathf.Min(sailAngularVelocity, (maxAngle - mSailAngle) ), -maxAngle - mSailAngle);
            sailAngularVelocity = Mathf.Max(sailAngularVelocity, -ropeClampMaxVelocity);

            sailMomentumPush -= sailAngularVelocity;


            angularAcceleration = Mathf.Clamp(angularAcceleration, -maxAngle, 0);

            if (mSailAngle < maxAngle)
                return;

            mSailAngle = sailAngle;
            mSailAngularVelocity = sailAngularVelocity;
            mAngularAcceleration = angularAcceleration;

            return;
        }



        sailMomentumPush = 0;
        pushAmount = 0;

    }

   

    private void ApplyAngularWindForce(float deltaTime)
    {


        sailWindDot = AngleDot(localWindAngle * Mathf.Deg2Rad, (sailSimulator.sailAngle + 90) * Mathf.Deg2Rad);


        float angularAcc = Mathf.Clamp(sailWindDot * sailAngularSpeedMult * windMagnitude, -maxAngle - sailSimulator.sailAngle, maxAngle - sailSimulator.sailAngle);

        sailSimulator.ApplyAngularAccelerationToSail(angularAcc * deltaTime);
    }

    private void ApplySailForceToBoat(float deltaTime)
    {
        appliedAcceleration = (Mathf.Pow(Mathf.Abs(pushAmount), effectiveAnglePow) 
            + Mathf.Abs(sailMomentumPush) * sailMomentumForceMult)
            * (pushAmount < 0 ? -1 : 1);


        Vector3 sailDirection = (sailSimulator.sailRotationWorld * Vector3.left);

        Debug.DrawRay(transform.position, sailDirection);

        appliedForce = appliedAcceleration * localWindMagnitude * sailForceFlag;

        var force = sailDirection * appliedForce;

        appliedSurge = Vector3.Dot(transform.forward, force);

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

