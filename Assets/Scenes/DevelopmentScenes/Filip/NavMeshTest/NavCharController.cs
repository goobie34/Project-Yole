using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NavCharController : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    private Vector3 inputValue = Vector3.zero;
    private InputAction moveAction;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void Update()
    {
        ReadInput();
        Step();
    }

    private void ReadInput()
    {
        if (moveAction == null)
            return;

        Vector2 input2D = moveAction.ReadValue<Vector2>();

        inputValue = new Vector3(input2D.x, 0f, input2D.y);
    }

    private void Step()
    {
        if (inputValue.sqrMagnitude < 0.01f)
            return;

        Vector3 currentPosition = transform.position - Vector3.up * 1f;

        if (!NavMesh.SamplePosition(currentPosition, out NavMeshHit currentHit, 0.3f, NavMesh.AllAreas))
        {
            return;
        }

        Vector3 movement = inputValue.normalized * speed * Time.deltaTime;

        Vector3 newPosition = currentHit.position + movement;

        if (NavMesh.Raycast(currentHit.position, newPosition, out NavMeshHit rayHit, NavMesh.AllAreas))
        {
            return;
        }

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit targetHit, 0.3f, NavMesh.AllAreas))
        {
            float heightDifference = Mathf.Abs(targetHit.position.y - currentHit.position.y);

            if (heightDifference > 0.3f)
                return;

            transform.position = targetHit.position + Vector3.up * 1f;
        }
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }
}
