using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NavCharController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 10f;
    [SerializeField]
    private float acceleration = 15f;
    [SerializeField]
    private float deceleration = 20f;
    [SerializeField]
    private Transform cameraTransform;

    [Header("NavMesh")]
    [SerializeField]
    private float sampleDistance = 0.3f;
    [SerializeField]
    private float maxHeightDifference = 0.3f;

    private Vector2 movementInput;
    public Vector3 currentVelocity;

    public Vector3 CurrentVelocity => currentVelocity;

    private void Start()
    {
        bool found = NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas);

        if (found)
        {
            Debug.Log("NavMesh found at: " + hit.position);
            Debug.Log("Player position: " + transform.position);
            Debug.Log("Distance: " + Vector3.Distance(transform.position, hit.position));

            transform.position = hit.position;
        }
        else
        {
            Debug.LogError("NO NAVMESH FOUND NEAR PLAYER!");
        }
    }

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (cameraTransform == null)
            return;

        //Get camera directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        //Calculate movement relative to camera
        Vector3 moveDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        //Calculate target velocity
        Vector3 targetVelocity = moveDirection * movementSpeed;

        float currentAcceleration = movementInput.sqrMagnitude > 0.001f ? acceleration : deceleration;
        
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, currentAcceleration * Time.deltaTime);

        MoveOnNavMesh();
    }

    private void MoveOnNavMesh()
    {
        if (currentVelocity.sqrMagnitude < 0.0001f)
            return;

        //Find current NavMesh position
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit currentHit, sampleDistance, NavMesh.AllAreas))
        {
            Debug.LogWarning("FAILED: Current position not on NavMesh");
            currentVelocity = Vector3.zero;
            return;
        }

        //Calculate desired movement
        Vector3 movement = currentVelocity * Time.deltaTime;

        Vector3 targetPosition = currentHit.position + movement;

        //Check if movement crosses a NavMesh boundary
        bool isBlocked = NavMesh.Raycast(currentHit.position, targetPosition, out NavMeshHit rayHit, NavMesh.AllAreas);

        if (isBlocked)
        {
            Debug.LogWarning("FAILED: NavMesh.RayCast blocked movement");
            currentVelocity = Vector3.zero;
            return;
        }

        //Find target position on NavMesh
        bool isValid = NavMesh.SamplePosition(targetPosition, out NavMeshHit targetHit, sampleDistance, NavMesh.AllAreas);

        if (!isValid)
        {
            Debug.LogWarning("FAILED: Target position not on NavMesh");
            currentVelocity = Vector3.zero;
            return;
        }

        //Prevent sudden vertical jumps
        float heightDifference = Mathf.Abs(targetHit.position.y - currentHit.position.y);

        if (heightDifference > maxHeightDifference)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        //Move player onto NavMesh
        transform.position = targetHit.position;
    }

    public void Move(Vector2 movementVector)
    {
        movementInput = Vector2.ClampMagnitude(movementVector, 1f);
    }    
}
