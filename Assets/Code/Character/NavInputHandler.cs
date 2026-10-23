using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class NavInputHandler : MonoBehaviour
{
    [SerializeField]
    private NavCharController character;

    private InputAction moveAction;

    private void Awake()
    {
        if (character == null)
            character = GetComponent<NavCharController>();

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
        if (moveAction == null || character == null)
            return;

        Vector2 movementVector = moveAction.ReadValue<Vector2>();

        if (movementVector.sqrMagnitude > 0.01f)
        {
            //Debug.Log("Input: " + movementVector);
        }

        character.Move(movementVector);
    }

    private void OnDisable()
    {
        if (character != null)
            character.Move(Vector2.zero);

        moveAction?.Disable();
    }
}
