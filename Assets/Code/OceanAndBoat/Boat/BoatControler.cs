using UnityEngine;
using UnityEngine.InputSystem;

public class BoatControler : MonoBehaviour
{
    public Rigidbody rb;
    public BoatVariableManager fuzzyFlags;
    private float boatMovementFlag => fuzzyFlags != null ? fuzzyFlags.Get_ControlBoat : 1;



    public float rotationSpeed = 1;
    public float f_movementSpeed = 10;
    public float r_movementSpeed = 4f;

    public float windMinRange = -0.5f;

   

    public Vector2 movementVector = Vector2.zero;
    public Vector2 currentInput = Vector2.zero;
    
    public float turnAccelTime = 1;
    public float turnDecelTime = 1;

    public float driveAccelTime = 1;
    public float driveDecelTime = 1;


    public Transform rudder;
    public float _maxRudderAngle = 70f;

    private Vector3 _windDirection = Vector3.zero;
    private float _windMagnitude = 0;

    public float _windFraction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        if(fuzzyFlags == null) fuzzyFlags = GetComponentInParent<BoatVariableManager>();


        if (rudder == null) rudder = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float deltaScaledLerp = inputLerp * (Time.fixedDeltaTime * 60);

       // this.currentInput = Vector2.Lerp(this.currentInput,movementVector, deltaScaledLerp);

        float turn = this.currentInput.x;
        float drive = this.currentInput.y;

        LerpHelper.InputSpeedAccelerationDriver(ref turn,   turnAccelTime,  turnDecelTime,  movementVector.x, Time.fixedDeltaTime);
        LerpHelper.InputSpeedAccelerationDriver(ref drive,  driveDecelTime, driveDecelTime, movementVector.y, Time.fixedDeltaTime);


        this.currentInput.x = turn;
        this.currentInput.y = drive;

        var currentInput = this.currentInput * boatMovementFlag;

        if (WindServiceLocator.Instance.TryGet(out var windService))
        {
            windService.EvaluateWind(transform.position, Time.fixedTime, out _windDirection, out _windMagnitude);
        }

        transform.RotateAround(rudder.position, Vector3.up, currentInput.x * rotationSpeed);
        if(rudder != null) rudder.localRotation = Quaternion.Euler(0, currentInput.x * _maxRudderAngle, 0);
        //_windFraction = Mathf.SmoothStep(1, windMinRange, Vector3.Dot(transform.forward, _windDirection)) * _windMagnitude;

        float currentSpeed = rb.linearVelocity.magnitude;

        float movementSpeed = currentInput.y > 0 ? f_movementSpeed : r_movementSpeed;

        // not used
        float adjustedAcceleration = currentInput.y * movementSpeed - ( Vector3.Dot(transform.forward,rb.linearVelocity));

        // not used
        if (currentInput.y * adjustedAcceleration <= 0)
            adjustedAcceleration = 0; 

        rb.AddForce(transform.forward * currentInput.y * movementSpeed * Time.fixedDeltaTime * 60, ForceMode.Acceleration);
    }

    

    private void OnMove(InputValue input)
    {
        movementVector = input.Get<Vector2>();

        
    }

}
