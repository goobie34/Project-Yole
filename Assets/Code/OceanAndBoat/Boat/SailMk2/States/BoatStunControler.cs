using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class BoatStunControler : MonoBehaviour
{


    public BoatVariableManager fuzzyFlags;

    public bool stunned = false;
    public bool stunnedAwaitingInput = false;


    public float minMovementToExitStunned = 0.5f;
    public float stunnedExitTimer = 1f;

    private float sumInput => sailInput + movementInput;
    private float movementInput;
    private float sailInput;

    private Coroutine stunCorutine;
    public Animator boat_animator;


    private void Start()
    {
        if (fuzzyFlags == null)
            fuzzyFlags = GetComponent<BoatVariableManager>();


        if (boat_animator == null)
            boat_animator = GetComponentInChildren<Animator>();


        StartCoroutine(CoroutineStunnedTimer(0));

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

        stunCorutine = null;

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

    public void StunBoat(float stunTimer)
    {
        if(stunCorutine != null)
        {
            StopCoroutine(stunCorutine);
            stunCorutine = null;
            Debug.Log("Stunn Corutine Stopped Midway");
        }

        boat_animator?.SetTrigger("Flash_Stunn");

        stunCorutine = StartCoroutine(CoroutineStunnedTimer(stunTimer));
    }
}