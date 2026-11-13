using UnityEngine;

public class SnakeingObject : MonoBehaviour
{
    public Transform target;

    public float targetLength = 3;
    public float lerp = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Lerp();
    }

    private void Lerp()
    {
        var targetPos = GetTargetPos(transform.position, target.position, targetLength);

        transform.position = Vector3.Lerp(transform.position, targetPos, lerp);
    }

    private Vector3 GetTargetPos(Vector3 A, Vector3 B, float targetDistance)
    {
        var direction = B - A;

        return A + direction.normalized * (direction.magnitude - targetDistance);
    }
}
