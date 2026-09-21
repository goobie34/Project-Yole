using System.Collections;
using UnityEngine;

public class SimpleWindService : MonoBehaviour, IWindService
{
    [SerializeField] private float _strength = 1f;

    public Vector3 windDirection => transform.forward;
    public float windStrength => _strength;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        WindServiceLocator.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        WindServiceLocator.Instance?.DeregisterIfThis(this);

    }


    public void EvaluateWind(Vector3 worldPos, float time, out Vector3 windDirection, out float windMagnitude)
    {
        windDirection = this.windDirection;
        windMagnitude = this.windStrength;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;

        Gizmos.DrawRay(transform.position, windDirection * windStrength);
        Gizmos.DrawSphere(transform.position + windDirection * windStrength, 0.1f);

        Gizmos.color = Color.white;
    }
}
