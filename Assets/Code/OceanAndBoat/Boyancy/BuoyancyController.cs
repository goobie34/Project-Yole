using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.LightAnchor;

public class BuoyancyController : MonoBehaviour
{
    public Rigidbody rb;

    public BoatRotationManager rotator;
    public BoatRotationManager bankingRotator;



    public float BuoyancyForce = 12;

    public float Size = 1;

    public float MinimumDepth = 0.5f;

    public bool doMinimumDepth = false;


    public float maxDampening = 0.8f;
    public float minDampening = 0.4f;

    public float aerodynamicTangentResistance = 1f;
    public float downAcc = 5;
    public float downPow = 2;

    public float normalRotationalForce = 0.1f;

    private float _waterHeight = 0f;
    private Vector3 _waterNormal = Vector3.up;

    public float submergedFrac = 0;
    private Vector2 GetWorldVec2Pos() => new Vector2(transform.position.x, transform.position.z);

    public float testForce = 10;

    public float bankingLeanForce = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null ) rb = GetComponent<Rigidbody>();
        if(rotator == null )
            rotator = GetComponentInChildren<BoatRotationManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!EvaluateWaterValues()) return;

        UpdateBoatPhysics();
    }

    private bool EvaluateWaterValues()
    {
        if (WaveServiceLocator.Instance.TryGet(out var waveManager))
        {
            waveManager.EvaluateWaves(GetWorldVec2Pos(), 1, out _waterHeight, out _waterNormal);
            return true;
        }

        return false;
    }

    private void UpdateBoatPhysics()
    {
        submergedFrac = CalculateSubmergedFraction(_waterHeight, rb.position.y, Size);

        var bouyancyForce = _waterNormal * submergedFrac * BuoyancyForce * rb.mass;

        rb.AddForce(bouyancyForce);





        rotator?.ApplyPullToNormal(transform.InverseTransformDirection(_waterNormal), normalRotationalForce * submergedFrac);
        //child.localRotation = Quaternion.Slerp(child.localRotation,  GetDirectionFromUpp(_waterNormal), submergedFrac * 0.5f);

        //rb.AddForce(child.forward * testForce);
        if (doMinimumDepth)
        {
            var f = BowForce(_waterNormal, rb.linearVelocity);
            f *= Unity.Mathematics.math.smoothstep(MinimumDepth, 1, submergedFrac);
            rb.AddForce(f, ForceMode.VelocityChange);


        }


        rb.linearDamping = Mathf.Lerp(minDampening,maxDampening, submergedFrac );

        //;

        //;

        rb.AddForce(Vector3.up * Mathf.Pow((1 - submergedFrac), downPow) * downAcc * Time.fixedDeltaTime * 60, ForceMode.Acceleration);

        var breakingForce = BreakingAcceleration(new Vector2(transform.forward.z, -transform.forward.x).normalized, new Vector2(rb.linearVelocity.x, rb.linearVelocity.z), 1, aerodynamicTangentResistance);

        float bankingLean = Vector3.Dot(transform.right, breakingForce);

        bankingRotator?.ApplyLocalAngularAcceleration(new Vector2(0, bankingLean * bankingLeanForce));


        var testValue = rb.angularVelocity + breakingForce;

        rb.AddForce(breakingForce * Time.fixedDeltaTime * 60, ForceMode.Acceleration);
    }

    private void BowForceLegacy()
    {
        if (doMinimumDepth && submergedFrac >= MinimumDepth)
        {
            rb.position += Vector3.up * Mathf.Pow((submergedFrac - MinimumDepth), 2) * Size;


            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, 0), rb.linearVelocity.z);
        }
    }

    private Vector3 BowForce(Vector3 waterNormal,Vector3 velocity)
    {
        return waterNormal *  -Mathf.Min(Vector3.Dot(velocity, waterNormal),0);
    }

   

    private Vector3 BreakingAcceleration(Vector2 tangent, Vector2 velocity, float submergedFrac, float aerodynamic)
    {
        float faceingFront = Vector3.Dot(tangent.normalized,velocity) * aerodynamic;

        float breakingFactor = -faceingFront * submergedFrac;

        var tangentBreakingForce = tangent * breakingFactor;

        return new Vector3(tangentBreakingForce.x, 0, tangentBreakingForce.y);
    }

    private float CalculateSubmergedFraction(float waterHeight, float currentHeight, float size)
    {
        float depth = currentHeight - waterHeight;

        float topDepth = depth + size / 2;

        float bottomDepth = depth - size / 2;

        return Mathf.Clamp01((bottomDepth / size) * -1);
    }

    private Quaternion GetDirectionFromUpp(Vector3 normal)
    {
        return Quaternion.FromToRotation(Vector3.up, transform.InverseTransformDirection(normal));
    }


    private void GizmosDrawWater()
    {
        Vector3 waterPos = new Vector3(transform.position.x, _waterHeight, transform.position.z);

        Gizmos.color = Color.limeGreen;

        Gizmos.DrawSphere(waterPos, 1);

        Gizmos.DrawRay(waterPos, _waterNormal);

        Gizmos.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        GizmosDrawWater();
    }


}
