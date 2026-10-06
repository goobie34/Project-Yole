using System;
using System.Security.Principal;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class BoatRotationManager : MonoBehaviour
{
    public Vector2 pitchRoll;

    public Vector2 angularVelocity; // pitch roll

    public Vector2 angularAcceleration;



    public Vector2 dampening = new Vector2(0.1f, 0.1f);

    public Vector2 inertia = new Vector2(50,50);
    
    private Vector2 _dampening => Vector2.one - dampening;

    public float uppForceStrength = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        ApplyLocalAngularAcceleration(AngularDifference(pitchRoll, Vector2.zero) * uppForceStrength);

        UpdatePhysics(Time.fixedDeltaTime * 60);

    }

    

    private void Update()
    {

        transform.localRotation = GetQuant();
    }

    private void UpdatePhysics(float deltaTime)
    {
        angularVelocity += angularAcceleration;

        pitchRoll += angularVelocity * deltaTime;

        angularVelocity *= _dampening;

        angularAcceleration = Vector2.zero;

        

    }

    public Vector2 AngularDifference(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.DeltaAngle(a.x,b.x), Mathf.DeltaAngle(a.y,b.y));
    }
    public void ApplyLocalAngularAcceleration(Vector2 anuglarDelta)
    {
        angularAcceleration += anuglarDelta / inertia;
    }
    
    public void ApplyPullToNormal(Vector3 normal, float strength = 1)
    {
        float pitch = Mathf.Asin(normal.z) * Mathf.Rad2Deg; //Vector3.SignedAngle(Vector3.up,normal, Vector3.right);// Mathf.Asin(normal.z) * Mathf.Rad2Deg;
        float roll = -Mathf.Asin(normal.x) * Mathf.Rad2Deg;// Vector3.SignedAngle(Vector3.up, normal, Vector3.forward); //Mathf.Acos(normal.x) * Mathf.Rad2Deg;

        var dif = Quaternion.FromToRotation(Vector3.up, normal);

        

        ApplyLocalAngularAcceleration(AngularDifference(pitchRoll, new Vector2(pitch, roll)) * strength);
    }

    private Quaternion GetQuant()
    {
        return quaternion.EulerXYZ(pitchRoll.x * Mathf.Deg2Rad, 0, pitchRoll.y * Mathf.Deg2Rad);
    }

    

}
