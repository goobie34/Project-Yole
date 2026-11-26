using UnityEngine;

public class PerimeterBoundsService : MonoBehaviour, IPerimeterBoundsService
{
    public float radius;
    private float targetRadius;

    public Vector3 origin;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PerimeterBoundsServiceLocator.Instance.Register(this);

        origin = Flatten(origin);

        targetRadius = radius;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
    
    public float GetRadius()
    {
        return radius;
    }

    public void SetOrigin(Vector3 center)
    {
        origin = center;
        origin = Flatten(origin);
    }

    public Vector3 GetOrigin()
    {
        return origin;
    }


    public float GetDistanceToEdge(Vector3 samplePos)
    {
        samplePos = Flatten(samplePos);

        float toCenter = Vector3.Distance(origin, samplePos);
        
        return radius - toCenter;
    }

    public Vector3 GetVectorToEdge(Vector3 samplePos)
    {
         var edgePos = GetClosestEdgePos(samplePos);

        return edgePos - samplePos;
    }

    public Vector3 GetClosestEdgePos(Vector3 samplePos)
    {
        var normal = GetClosestEdgeNormal(samplePos);

        var result = -normal * radius + origin;
        result.y = samplePos.y;

        return result;
    }

    public Vector3 GetClosestEdgeNormal(Vector3 samplePos)
    {
        samplePos = Flatten(samplePos);

        var relative = samplePos - origin;

        return -relative.normalized;
    }

    private Vector3 Flatten(Vector3 samplePos)
    {
        return new Vector3(samplePos.x, 0, samplePos.z);
    }


    


    public Coroutine TransitionToRadius(float targetRadius, float duration)
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(origin, radius);

        if (targetRadius == radius)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, targetRadius);
        
        Gizmos.color = Color.white;
    }
}
