using UnityEngine;
using UnityEngine.InputSystem;

public class SailRopeControler : MonoBehaviour
{
    [Header("Rope Parameters")]
    public float minRopeLength;
    public float maxRopeLength;

    public float extendSpeed = 0.1f;
    public float retractSpeed = 0.1f;

    public float minExtendLerp = 0.5f;
    public float minRetractLerp = 0.5f;


    public float accelTime = 1f;
    public float decelTime = 1f;


    //public float inputLerp = 0.5f;



    [Header("Dependencies")]
    public SailingForceControler sailControler;

    public BoatVariableManager boatFlags;
    private float sailControlFlag => boatFlags != null ? boatFlags.Get_ControlMainsailRope : 1;

    [Header("Data")]
    public float targetInputValue;
    public float currentInputValue;

    public float ropeLength;

    private void Start()
    {
        if(boatFlags == null)
            boatFlags = GetComponentInParent<BoatVariableManager>();

        if (sailControler == null)
            sailControler = GetComponent<SailingForceControler>();
    }

    private void OnExtendSail(InputValue input)
    {
        targetInputValue = input.Get<float>();

    }

    private void FixedUpdate()
    {


        float deltaTime = Time.fixedDeltaTime * 50;

        LerpHelper.InputSpeedAccelerationDriver(ref currentInputValue, accelTime, decelTime, targetInputValue, Time.fixedDeltaTime);

        ropeLength = sailControler.mainsailRopeLength;

        float clamedInputValue = currentInputValue * sailControlFlag;
        if (ropeLength >= maxRopeLength)
            clamedInputValue = Mathf.Min(clamedInputValue, 0);

        if (ropeLength <= minRopeLength)
            clamedInputValue = Mathf.Max(clamedInputValue, 0);




        ExtendRope(clamedInputValue, deltaTime);
        PullInRope(-clamedInputValue, deltaTime);


        ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);


        if (sailControler == null)
            return;

        sailControler.mainsailRopeLength = ropeLength;
    }

    private void ExtendRope(float inputSpeed, float scaledDeltaTime)
    {
        inputSpeed = Mathf.Max(inputSpeed, 0);

        float sailForce = Mathf.Abs(sailControler != null ? sailControler.appliedAcceleration : 1);

        sailForce = Mathf.Lerp(minExtendLerp, 1, sailForce);


        float ropeForce = extendSpeed * sailForce * scaledDeltaTime * inputSpeed;


        ropeLength += ropeForce;

    }

    private void PullInRope(float inputSpeed, float scaledDeltaTime)
    {
        inputSpeed = Mathf.Max(inputSpeed, 0);

        float sailForce = Mathf.Abs(sailControler != null ? sailControler.appliedAcceleration : 1);

        sailForce = Mathf.Lerp(1, minRetractLerp, sailForce);

        float ropeForce = retractSpeed * sailForce * scaledDeltaTime * inputSpeed;

        ropeLength -= ropeForce;
    }
    


}
