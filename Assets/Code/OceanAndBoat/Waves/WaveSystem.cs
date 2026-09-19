using UnityEngine;

public class WaveSystem : MonoBehaviour, IWaveService
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        WaveServiceLocator.Instance?.Register(this);
    }

    void OnDisable()
    {
        WaveServiceLocator.Instance?.DeregisterIfThis(this);
    }

    public float EvaluateHeight(Vector2 worldPos)
    {
        return 0;
    }

    public Vector3 EvaluateNormal(Vector3 worldPos)
    {
        return Vector3.up;
    }

}
