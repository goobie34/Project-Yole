using UnityEngine;

public class RotateToVelocity : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 0.8f;
    public float minThreshold = 1;
    public float minThreshold_Smoothness = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null)
            rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        

        var look = Quaternion.LookRotation(rb.linearVelocity, Vector3.up);
        var current = rb.rotation;

        var lerp = Quaternion.Lerp(current, look, speed * Unity.Mathematics.math.smoothstep(minThreshold * minThreshold_Smoothness, minThreshold, rb.linearVelocity.magnitude));

        rb.MoveRotation(lerp);
    }


}
