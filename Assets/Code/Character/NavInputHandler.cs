using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class NavInputHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerStateDriver stateDriver;

    private InputAction moveAction;

    private void Awake()
    {
        if (stateDriver == null)
            stateDriver = GetComponent<PlayerStateDriver>();

        moveAction = InputSystem.actions?.FindAction("Player/Move");

        if (moveAction == null)
        {
            Debug.LogError("Move Input Action not found!");
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void Update()
    {
        if (moveAction == null || stateDriver == null)
            return;

        Vector2 movementVector = moveAction.ReadValue<Vector2>();

        if (movementVector.sqrMagnitude > 0.01f)
        {
            //Debug.Log("Input: " + movementVector);
        }

        stateDriver.SetMovementInput(movementVector);
    }

    private void OnDisable()
    {
        if (stateDriver != null)
            stateDriver.SetMovementInput(Vector2.zero);

        moveAction?.Disable();
    }
}
