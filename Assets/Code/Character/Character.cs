using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private CharacterController characterController;
    private EdgeDetection edgeDetection;

    [Header("Movement")]
    [SerializeField] 
    private float movementSpeed = 10f;

    [SerializeField]
    private float acceleration = 15f;

    [SerializeField]
    private float deceleration = 20f;

    [SerializeField]
    private Transform cameraTransform;

    [Header("Gravity")]
    [SerializeField]
    public float gravity = -22.5f;

    
    private float verticalVelocity;
    private Vector2 movementInput;
    private Vector3 currentVelocity;

    public Vector3 CurrentVelocity => currentVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        edgeDetection = GetComponent<EdgeDetection>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        edgeDetection = GetComponent<EdgeDetection>();
    }

    void Update()
    {
        ApplyGravity();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        if (cameraTransform == null)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        Vector3 targetVelocity = moveDirection * movementSpeed;

        float currentAcceleration = movementInput.sqrMagnitude > 0.001f ? acceleration : deceleration;

        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, currentAcceleration * Time.deltaTime);

        //Edge detection
        Vector3 horizontalMovement = currentVelocity * Time.deltaTime;

        if (edgeDetection != null && characterController.isGrounded && horizontalMovement.sqrMagnitude > 0.0000001f && !edgeDetection.IsGroundAhead(horizontalMovement))
        {
            currentVelocity = Vector3.zero;
        }

        //Combine horizontal movement and gravity
        Vector3 finalMovement = currentVelocity;
        finalMovement.y = verticalVelocity;

        characterController.Move(finalMovement * Time.deltaTime);       
    }

    public void Move(Vector2 movementVector)
    {      
        movementInput = Vector2.ClampMagnitude(movementVector, 1f);
    }   
}
