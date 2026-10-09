using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private CharacterController characterController;
    private EdgeDetection edgeDetection;

    [SerializeField]
    public float movementSpeed = 10f;
    [SerializeField]
    public float rotationSpeed = 5f;
    [SerializeField]
    public float gravity = -38f;

    private float rotationY;
    private float verticalVelocity;
    private Vector2 movementInput;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        edgeDetection = GetComponent<EdgeDetection>();
    }

    void Update()
    {      
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = transform.forward * movementInput.y + transform.right * movementInput.x;

        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 horizontalMovement = move * movementSpeed * Time.deltaTime;

        if (characterController.isGrounded && horizontalMovement.sqrMagnitude > 0.0000001f && !edgeDetection.IsGroundAhead(horizontalMovement))
        {
            move = Vector3.zero;
        }       

        move *= movementSpeed;

        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);
    }

    public void Move(Vector2 movementVector)
    {
        //Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        //move = move * movementSpeed * Time.deltaTime;
        //characterController.Move(move);
        movementInput = movementVector;
    }

    public void Rotate(Vector2 rotationVector)
    {
        rotationY += rotationVector.x * rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
}
