using UnityEngine;

public class BoatControler : MonoBehaviour
{

    private float placeholderHeight = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(WaveServiceLocator.Instance.TryGet(out var waveManager)){
            waveManager.EvaluateWaves(new Vector2(transform.position.x, transform.position.z), 1, out placeholderHeight, out Vector3 normal);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, placeholderHeight, transform.position.z), 1);
    }
}
