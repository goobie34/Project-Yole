using UnityEngine;

public class SailPhysics : MonoBehaviour
{
    [Header("Configs")]
    public float sailInertia = 1f;
    public float sailAngularDampening = 0.5f;
    public Transform RotatorObject;

    [Header("Data")]
    public float sailAngle;
    public float sailAngularVelocity;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePhysics(Time.fixedDeltaTime);
    }

    private void UpdatePhysics(float deltaTime)
    {


        sailAngle += sailAngularVelocity * deltaTime;

        sailAngularVelocity *= sailAngularDampening;


    }

    public void ApplyAngularAccelerationToSail(float acceleration)
    {

    }
}
