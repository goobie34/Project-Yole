using UnityEngine;

public class WaveServiceLocator : ServiceLocatorPersistent<IWaveService> { }

public interface IWaveService
{
    public float EvaluateHeight(Vector2 worldPos);

    public Vector3 EvaluateNormal(Vector3 worldPos);
}