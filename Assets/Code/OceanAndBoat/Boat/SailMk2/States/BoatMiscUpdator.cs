using System;
using Unity.Cinemachine;
using UnityEngine;

public class BoatMiscUpdator : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    public BoatVariableManager variableManager;

    private void Start()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();

        if (variableManager == null)
            variableManager = GetComponent<BoatVariableManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float movementVariable =  1 - variableManager.Get_FreeMovement;

        if(movementVariable <= 0.0001f)
            return;

        var velocityXZ = -rb.linearVelocity; velocityXZ.y = 0;

        velocityXZ *= movementVariable;

        rb.AddForce(velocityXZ, ForceMode.VelocityChange);
    }

    
}
