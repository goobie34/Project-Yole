using UnityEngine;

public class PerimeterBoundsServiceLocator : ServiceLocator<IPerimeterBoundsService>
{
   
}

public interface IPerimeterBoundsService
{
    public void SetRadius(float radius);
    public Coroutine TransitionToRadius(float targetRadius, float duration);
    public float GetRadius();

    public void SetOrigin(Vector3 center);

    public Vector3 GetOrigin();

    public float GetDistanceToEdge(Vector3 samplePos);

    public Vector3 GetVectorToEdge(Vector3 samplePos);

    public Vector3 GetClosestEdgePos(Vector3 samplePos);

    public Vector3 GetClosestEdgeNormal(Vector3 samplePos);


}