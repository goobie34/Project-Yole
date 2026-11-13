using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class CaravanPhysManager : MonoBehaviour
{


    public Transform rootObject;
    public Rigidbody rb;
    public List<Rigidbody> snake;

    public float targetDist = 3;
    public float forcePow = 2;
    public float retractForceMult = 3;

    public float extendForceMult = 0.1f;

    public float overLeniancy = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(rb == null)
            rb = GetComponentInParent<Rigidbody>();

        if(rootObject == null)
            rootObject = transform;

        
    }

    private void OnDestroy()
    {
        for(int i = 0; i < snake.Count; i++)
        {
            if (snake[i] == null)
                continue;
            Destroy(snake[i].gameObject);
        }

        snake.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateTrail(Time.fixedDeltaTime * 50);
    }

    void UpdateTrail(float delaTime)
    {
        if(snake == null)
            return;

        if(snake.Count <= 0)
            return;

        UpdateSnakeObject(snake[0], rootObject, rb, targetDist, delaTime);

        for (int i = 1; i < snake.Count; i++)
        {
            UpdateSnakeObject(snake[i], snake[i - 1].transform, snake[i - 1], targetDist, delaTime);

        }

    }

    public void UpdateSnakeObject(Rigidbody rb, Transform target, Rigidbody targetRB, float targetLength, float detlaTime)
    {
        var delta =  target.position - rb.position;

        var normal = delta.normalized;
        var mag = delta.magnitude;

        var overDistance = -(targetLength - mag);


        if(overDistance >= overLeniancy)
        {
            var targetPos = rb.position + normal * (overDistance - overLeniancy);


            var dtPos = Vector3.MoveTowards(rb.position, targetPos, overDistance * detlaTime);
            Debug.DrawLine(rb.position, dtPos);

            rb.MovePosition(targetPos);

            

            //rb.AddForce(normal * force * detlaTime);

        }

        if(overDistance >= 0)
        {
            var parentForce = targetRB.GetPointVelocity(target.position);

            float force = Vector3.Dot(normal, rb.linearVelocity  - parentForce);
            
            var forceV = -normal * force * detlaTime;

            Debug.DrawRay(rb.position, forceV, Color.red);

            rb.AddForce(forceV, ForceMode.Acceleration);
        }

        //var springforce = targetLength - mag;

        //springforce = Mathf.Pow(Mathf.Abs(springforce), forcePow) * ((springforce < 0) ? -retractForceMult : extendForceMult);


        var p1 = rb.position;
        var p2 = target.position;
        p1.y = 0;
        p2.y = 0;

        rb.rotation = Quaternion.LookRotation(p2 - p1, Vector3.up);
    }

    public void AddItemToSnake(Rigidbody rb)
    {
        if (snake == null)
        {
            snake = new();
        }
           

        Transform prev = rootObject;

        if(snake.Count != 0)
        {
            prev = snake[snake.Count - 1].transform;
        }

        rb.position = prev.position + Random.insideUnitSphere;

        snake.Add(rb);
        
    }
}
