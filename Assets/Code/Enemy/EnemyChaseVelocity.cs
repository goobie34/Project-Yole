using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyChaseVelocity : MonoBehaviour
{
    private Rigidbody rigidBody;
    public Transform target;
    public float maxSpeed;
    public float maxSteering;
    public float threshold_One;
    public float targetHeight;

    void Start()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        rigidBody.AddForce(SeekBehavior(target.position, maxSpeed, maxSteering), ForceMode.Acceleration);

        LerpHeight();
    }

    private void StateMachine()
    {
        var targetPos = target.position;
        var distance = Vector3.Distance(rigidBody.position, targetPos);

        if (threshold_One > distance)
        {

        }
    }

    private Vector3 SeekBehavior(Vector3 targetPos, float maxSpeed, float maxSteering)
    {
        var desiredVelocity = -(rigidBody.position - targetPos).normalized * maxSpeed;
        var steering = desiredVelocity - rigidBody.linearVelocity;
        steering = Vector3.ClampMagnitude(steering, maxSteering);

        Debug.DrawRay(transform.position, desiredVelocity, Color.yellow);
        Debug.DrawRay(transform.position + rigidBody.linearVelocity, steering, Color.green);
        Debug.DrawRay(transform.position, rigidBody.linearVelocity, Color.teal);

        //if (rigidBody.linearVelocity.magnitude > 1)
        //{
        //    steering = Vector3.ProjectOnPlane(steering, rigidBody.linearVelocity.normalized);
        //}
        
        steering.y = 0;

        return steering;
    }

    private void LerpHeight()
    {
        var pos = rigidBody.position;
        pos.y = targetHeight;
        rigidBody.position = pos;
    }
}

//desired_velocity = normalize (position - target) * max_speed
//steering = desired_velocity - velocity
