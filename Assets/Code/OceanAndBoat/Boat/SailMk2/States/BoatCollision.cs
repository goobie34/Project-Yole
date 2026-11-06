using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatCollision : MonoBehaviour
{
    public float minCollisionSpeed = 4;
    public float minMovementToExitStunned = 0.5f;

    public float stunnedTimer = 2;
    public float stunnedExitTimer = 1f;

    public BoatVariableManager fuzzyFlags;
    public Animator boat_animator;

    public bool stunned = false;
    public bool stunnedAwaitingInput = false;

    private float sumInput => sailInput + movementInput;
    private float sailInput;
    private float movementInput;

    

    private void Start()
    {
        if(fuzzyFlags == null)
            fuzzyFlags = GetComponent<BoatVariableManager>();

        if(boat_animator == null )
            boat_animator = GetComponentInChildren<Animator>();

        StartCoroutine(CoroutineStunnedTimer(0));

    }

    private void OnCollisionEnter(Collision collision)
    {
        var velocity = collision.relativeVelocity;

        float directionalSpeed = velocity.magnitude; //Vector3.Dot(collision.contacts[0].normal, velocity);


        if (fuzzyFlags.Get_UnStunnable >= 1)
            return;

        if(directionalSpeed >= minCollisionSpeed)
            TriggerCollision(directionalSpeed);
    }

    private void TriggerCollision(float relativeSpeed, bool triggerFlash = true)
    {
        Debug.Log($"Collision, speed: {relativeSpeed}");

        if(triggerFlash )
            boat_animator?.SetTrigger("Flash_Stunn");

        StartCoroutine(CoroutineStunnedTimer(stunnedTimer));
    }

    private IEnumerator CoroutineStunnedTimer(float duration)
    {
        stunned = true;
        fuzzyFlags.Set_Stunned.Set(1);

        yield return new WaitForSeconds(duration);

        stunned = false;

        stunnedAwaitingInput = true;

        Debug.Log("Stunned Boat Waiting for Input");

        while (sumInput < minMovementToExitStunned)
        {
            yield return null;
        }

        Debug.Log("Stunned Boat Starting Exit Timer");

        float timer = 0;

        fuzzyFlags.Set_Stunned.Set(1);
        while (timer <= stunnedExitTimer)
        {
            timer += Time.smoothDeltaTime;
            fuzzyFlags.Set_Stunned.Set(1 - Mathf.Clamp01(timer / stunnedExitTimer));
            yield return null;
        }

        fuzzyFlags.Set_Stunned.Set(0);


        Debug.Log("Boat Exit Stunned State");
    }

    private void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>().magnitude;
    }

    private void OnExtendSail(InputValue input)
    {
        sailInput = input.Get<float>();

    }

}
