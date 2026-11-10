using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatDockingControler : MonoBehaviour
{
    private LinkedList<DockingManager> activeManagers = new LinkedList<DockingManager>();

    public bool IsByDock => activeManagers.Count > 0;

    private float inputVal;

    private Coroutine lerpCorutine;

    public float LerpTimer = 3f;

    public float turnAroundThreshold = 4f;

    public BoatVariableManager fuzzyFlags;
    public Rigidbody rb;

    [Header("Curves")]

    public AnimationCurve positionLerpCurve;
    public AnimationCurve rotationLerpCurve;
    public AnimationCurve freeLerpCurve;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fuzzyFlags == null)
            fuzzyFlags = GetComponent<BoatVariableManager>();

        if(rb == null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ToggleDockSequence();
    }

    private void ToggleDockSequence()
    {
        if (!IsByDock)
            return;

        if(lerpCorutine == null && inputVal > 0)
        {
            StartLerp();
            return;
        }

        if(lerpCorutine != null && inputVal == 0)
        {
            EndLerp();
            return;
        }
        

        
    }

    private void OnDock(InputValue input)
    {
        inputVal = input.Get<float>();
    }

    private IEnumerator LerpCorutine(float time)
    {
        FindClosestDockingManager(out var closestDock, out var dockIndex);

        closestDock.dockIndex = dockIndex;
        //closestDock.reversed = Vector3.Dot(transform.forward, closestDock.GetTransformAtIndex(dockIndex).forward) < 0;

        var dockPos = closestDock.GetTransformAtIndex(dockIndex).position;
        var dockToBoatD = (transform.position - dockPos).normalized;
        var dockFWD = closestDock.GetTransformAtIndex(dockIndex).forward;

        if(Vector3.Distance(transform.position, dockPos) < turnAroundThreshold)
        {
            closestDock.reversed = Vector3.Dot(transform.forward, dockFWD) < 0;
        } 
        else
        {
            closestDock.reversed = Vector3.Dot(dockToBoatD, dockFWD) > 0;

        }




        float timer = 0f;

        float lerp = 0;

        var startPos = transform.position;
        var startYaw = transform.rotation.eulerAngles.y;

        var target = closestDock.GetCurrentDockPos;
        var reversed = closestDock.reversed;


        while (timer < time)
        {
            lerp = timer / time;
            lerp = Mathf.Clamp01(lerp);


            timer += Time.deltaTime;

            float positionLerp = positionLerpCurve.Evaluate(lerp);
            float rotationLerp = rotationLerpCurve.Evaluate(lerp);
            float freeLerp = freeLerpCurve.Evaluate(lerp);


            UpdateLerp(positionLerp, rotationLerp, startPos, startYaw, target, reversed, out var outPos, out var outYaw);



            fuzzyFlags?.Set_Docked.Set(freeLerp);


            rb.transform.position = Vector3.Lerp(rb.transform.position, outPos, freeLerp);
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, Quaternion.Euler(0,outYaw,0), freeLerp);

            yield return null;
        }

        lerp = 0;

        closestDock.StartIslandState();
    }

    private void UpdateLerp(float positionLerp, float rotationLerp, Vector3 startPos, float startYaw, Transform target, bool reversed, out Vector3 outPos, out float outYaw )
    {
        

        startPos.y = 0;
        Vector3 targetPos = target.position;
        targetPos.y = 0;

        var lerpPos = Vector3.Lerp(startPos, targetPos, positionLerp);
        lerpPos.y = rb.position.y;

        outPos = lerpPos;

        float targetRot = target.rotation.eulerAngles.y;
        if (reversed)
            targetRot += 180;

        float lerpYaw = Mathf.LerpAngle(startYaw, targetRot, rotationLerp);
        outYaw = lerpYaw;



    }

    


    private void StartLerp()
    {
        if(lerpCorutine != null)
        {
            StopCoroutine(lerpCorutine);

            lerpCorutine = null;
        }


        lerpCorutine = StartCoroutine(LerpCorutine(LerpTimer));
    }

    

    private void EndLerp()
    {
        if (lerpCorutine != null)
        {
            StopCoroutine(lerpCorutine);

            lerpCorutine = null;
        }

        fuzzyFlags?.Set_Docked.Set(0);

    }



    private void FindClosestDockingManager(out DockingManager closestDock, out int closestIndex)
    {

        closestDock = null;
        closestIndex = -1;

        Transform closestTransform = null;

        float closestDistance = -1f;

        foreach(var manager  in activeManagers)
        {
            manager.GetClosestDockingIndex(transform, out var index);
            var current = manager.GetTransformAtIndex(index);

            float distance = Vector3.Distance(current.position, transform.position);

            if(closestDistance < 0)
            {
                closestDistance = distance;
                closestTransform = current;

                closestDock = manager;
                closestIndex = index;
                continue;
            }
            
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = current;

                closestDock = manager;
                closestIndex = index;
                continue;
            }

        }
    }



    private void OnTriggerEnter(Collider other)
    {
        var dockingManager = other.GetComponent<DockingManager>();

        if (dockingManager == null)
            return;

        activeManagers.AddLast(dockingManager);

        Debug.Log("Boat Enterd Dock Trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        var dockingManager = other.GetComponent<DockingManager>();

        if (dockingManager == null)
            return;

        activeManagers.Remove(dockingManager);
        Debug.Log("Boat Exited Dock Trigger");
    }

    
}
