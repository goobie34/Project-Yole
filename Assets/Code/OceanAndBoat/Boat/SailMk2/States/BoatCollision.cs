using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatCollision : MonoBehaviour
{
    public float minCollisionSpeed = 4;

    public BoatVariableManager fuzzyFlags;
    public Animator boat_animator;

    public BoatStunControler stun_controler;

    public bool stunned = false;
    public bool stunnedAwaitingInput = false;


    public float stunnedTimer = 2;



    private void Start()
    {
        if(fuzzyFlags == null)
            fuzzyFlags = GetComponent<BoatVariableManager>();

        if(boat_animator == null )
            boat_animator = GetComponentInChildren<Animator>();



        if(stun_controler == null )
            stun_controler = GetComponentInParent<BoatStunControler>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        var velocity = collision.relativeVelocity;

        float directionalSpeed = velocity.magnitude; //Vector3.Dot(collision.contacts[0].normal, velocity);


        if (fuzzyFlags.Get_UnStunnable >= 0.5f)
            return;

        if(directionalSpeed >= minCollisionSpeed)
            TriggerCollision(directionalSpeed);
    }

    private void TriggerCollision(float relativeSpeed, bool triggerFlash = true)
    {
        Debug.Log($"Collision, speed: {relativeSpeed}");


        stun_controler?.StunBoat(stunnedTimer);
    }

    

}
