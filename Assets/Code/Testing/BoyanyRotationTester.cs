using UnityEngine;

public class BoyanyRotationTester : MonoBehaviour
{

    public float strength;
    public Transform normal;

    public BoatRotationManager rotator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(normal == null)
            normal = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotator.ApplyPullToNormal(normal.up, strength);
    }

    private void OnDrawGizmos()
    {
        if(normal == null )
            return;

        Gizmos.DrawRay(transform.position,rotator.transform.up);
        Gizmos.DrawRay(transform.position, normal.up);
    }
}
