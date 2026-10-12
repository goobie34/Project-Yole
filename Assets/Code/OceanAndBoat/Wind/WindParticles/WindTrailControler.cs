using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class WindTrailControler : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float minHeight;

    [SerializeField] private float minHeightForce = 0.01f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (WindServiceLocator.Instance == null)
            return;

        if (!WindServiceLocator.Instance.TryGet(out var windService))
            return;

        ApplyWindAcceleration(windService);
        ApplyHeightForce();
    }

    private void ApplyWindAcceleration(IWindService windService)
    {
        

        windService.EvaluateWind(transform.position, Time.time, out var windDirection, out var windMagnitude);

        var windForceMag = Mathf.Max(windMagnitude - Vector3.Dot(windDirection, rb.linearVelocity), 0);

        

        rb.AddForce(new Vector3(windDirection.x,0,windDirection.z).normalized * windForceMag, ForceMode.Acceleration);
    }

    private void ApplyHeightForce()
    {
        if (transform.position.y > minHeight)
            return;

        float distance = minHeight - transform.position.y;

        rb.AddForce(Vector3.up * distance * minHeightForce, ForceMode.Acceleration);
    }

    private void ApplyNoise()
    {

    }
}
