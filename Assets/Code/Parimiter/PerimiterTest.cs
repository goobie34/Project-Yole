using UnityEngine;

public class PerimiterTest : MonoBehaviour
{
    public float    radius;
    public Vector3  origin;
    public float    distToEdge;
    public Vector3  vToEdge;
    public Vector3  edgePos;
    public Vector3  edgeNormal;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PerimeterBoundsServiceLocator.Instance == null)
            return;

        if (!PerimeterBoundsServiceLocator.Instance.TryGet(out var service))
            return;

        distToEdge  = service.GetDistanceToEdge(transform.position);
        vToEdge     = service.GetVectorToEdge(transform.position);
        edgePos     = service.GetClosestEdgePos(transform.position);
        edgeNormal  = service.GetClosestEdgeNormal(transform.position);

        radius      = service.GetRadius();
        origin      = service.GetOrigin();
    }

    private void OnDrawGizmosSelected()
    {
        

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(edgePos, edgeNormal);
        Gizmos.DrawSphere(edgePos + edgeNormal, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(edgePos, 1);

        Gizmos.color = distToEdge >= 0 ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, vToEdge);

        Gizmos.color = Color.white;


        Gizmos.matrix = Matrix4x4.TRS(origin + Vector3.up * transform.position.y, Quaternion.identity, new Vector3(1, 0, 1));

        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = Matrix4x4.identity;
    }

}
