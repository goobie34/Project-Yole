using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Character character;

    public InputAction moveAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        
        if (moveAction == null)
        {
            Debug.LogError("Move Input Action not found!");
        }
    }

    
    void Update()
    {
        if (moveAction == null || character == null)
            return;

        Vector2 movementVector = moveAction.ReadValue<Vector2>();

        character.Move(movementVector);
    }
}
