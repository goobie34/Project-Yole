using UnityEngine;
using UnityEngine.InputSystem;

public class BoatControler : MonoBehaviour
{
    public Rigidbody rb;

  

    public float rotationSpeed = 1;
    public float movementSpeed = 1500;

    public float windMinRange = -0.5f;

   

    public Vector2 movementVector = Vector2.zero;
    public Vector2 currentInput = Vector2.zero;
    public float inputLerp = 0.5f;


    public Transform rudder;

    private Vector3 _windDirection = Vector3.zero;
    private float _windMagnitude = 0;

    public float _windFraction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();


        if (rudder == null) rudder = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentInput = Vector2.Lerp(currentInput,movementVector,inputLerp);

        if(WindServiceLocator.Instance.TryGet(out var windService))
        {
            windService.EvaluateWind(transform.position, Time.fixedTime, out _windDirection, out _windMagnitude);
        }

        transform.RotateAround(rudder.position, Vector3.up, currentInput.x * rotationSpeed);

        //_windFraction = Mathf.SmoothStep(1, windMinRange, Vector3.Dot(transform.forward, _windDirection)) * _windMagnitude;

        rb.AddForce(transform.forward * currentInput.y * movementSpeed * Time.fixedDeltaTime * 60, ForceMode.Acceleration);
    }

    

    private void OnMove(InputValue input)
    {
        movementVector = input.Get<Vector2>();

        
    }

}
