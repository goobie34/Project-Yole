using UnityEngine;
using System.Collections.Generic;
public class CaravanPhysManager : MonoBehaviour
{
    public Transform rootObject;
    public List<Rigidbody> snake;

    public float targetDist = 3;
    public float forcePow = 2;
    public float forceMult = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        UpdateSnakeObject(snake[0], rootObject, targetDist, delaTime);

        for (int i = 1; i < snake.Count; i++)
        {
            UpdateSnakeObject(snake[i], snake[i - 1].transform, targetDist, delaTime);

        }

    }

    public void UpdateSnakeObject(Rigidbody rb, Transform target, float targetLength, float detlaTime)
    {
        var delta = rb.transform.position - target.position;

        var normal = delta.normalized;
        var mag = delta.magnitude;

        var force = targetLength - mag;

        force = Mathf.Pow(Mathf.Abs(force), forcePow) * forceMult * ((force < 0) ? -1 : 1);

        rb.AddForce(normal * force * detlaTime);

        var p1 = rb.position;
        var p2 = target.position;
        p1.y = 0;
        p2.y = 0;

        rb.rotation = Quaternion.LookRotation(p2 - p1, Vector3.up);
    }
}
