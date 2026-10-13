using UnityEngine;

public class SailPhysics : MonoBehaviour
{
    [Header("Configs")]
    public float sailInertia = 1f;
    public float sailAngularDampening = 0.5f;
    public Transform boatRotationReference;

    [Header("Data")]
    public float sailAngle;
    public float sailAngularVelocity;
    public float sailAngularAcceleration;
    public Quaternion sailRotation { get; private set; }

    public Quaternion sailRotationWorld => sailRotation * boatRotationReference.rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        UpdatePhysics(Time.fixedDeltaTime * 60);

        sailRotation =  Quaternion.Euler(0, sailAngle, 0);
    }

    public void UpdatePhysics(float deltaTime)
    {
        //sailAngularVelocity += sailAngularAcceleration * deltaTime;

        sailAngle += sailAngularVelocity * deltaTime;

        sailAngularVelocity *= 1 - sailAngularDampening;

        sailAngularAcceleration = 0;

    }

    public void ApplyAngularAccelerationToSail(float acceleration)
    {
        sailAngularVelocity += acceleration / sailInertia;
    }
}
