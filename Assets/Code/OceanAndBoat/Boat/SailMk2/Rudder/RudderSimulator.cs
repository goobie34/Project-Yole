using UnityEngine;
using UnityEngine.InputSystem;

public class RudderSimulator : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    public Transform rudderTransform;


    [Header("Variables")]
    public float rudderMomentArm = 0.5f;

    public float angularDampening = 0.8f;
    public float getAngularDampening => 1 - angularDampening;

    public float rudderForce = 5;

    public float rudderAngle = 0;    

    public Vector2 localRudderDirection => new Vector2(Mathf.Cos(rudderAngle * Mathf.Deg2Rad), Mathf.Sin(rudderAngle * Mathf.Deg2Rad));
    public Vector2 localRudderTangent => new Vector2(Mathf.Sin(rudderAngle * Mathf.Deg2Rad), Mathf.Cos(rudderAngle * Mathf.Deg2Rad));

    public float maxAngle = 40f;

    public float currentInput = 0;
    public float targetInput = 0;

    //public float inputLerp = 0.05f;
    public float accelTime = 1f;
    public float decelTime = 1f;

    [Header("Data")]

    public float rudderLiftVector;
    public float boatAngularVelocity = 0f;

    public Vector2 localVelocity;






    private void Start()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LerpHelper.InputSpeedAccelerationDriver(ref currentInput, accelTime, decelTime, targetInput, Time.fixedDeltaTime);

        rudderAngle = currentInput * maxAngle;

        var localVelocityV3 =  transform.InverseTransformDirection(rb.linearVelocity);

        localVelocity = new Vector2(localVelocityV3.x, localVelocityV3.z);

        CalculateRudderLift();
        UpdatePhysics(Time.fixedDeltaTime);
        ApplyRotation();

        if (rudderTransform != null) rudderTransform.localRotation = Quaternion.Euler(0, -rudderAngle, 0);
    }

    private void CalculateRudderLift()
    {
        rudderLiftVector = Vector2.Dot(localRudderDirection, localVelocity)
            * Vector2.Dot(Vector2.up, localRudderTangent);
    }

    
    private void UpdatePhysics(float deltaTime)
    {
        boatAngularVelocity += rudderLiftVector * Time.fixedDeltaTime * rudderMomentArm * rudderForce;

        boatAngularVelocity *= getAngularDampening;
    }

    private void ApplyRotation()
    {
        transform.RotateAround(rudderTransform.position, Vector3.up, boatAngularVelocity);

    }

    private void OnMove(InputValue input)
    {
        targetInput = input.Get<Vector2>().x;


    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position + transform.forward * -rudderMomentArm, transform.right * boatAngularVelocity);
       
    }
}
