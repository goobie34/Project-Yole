using UnityEngine;

public class WaveServiceLocator : ServiceLocator<IWaveService> { }

public interface IWaveService
{
    public void EvaluateWaves(Vector2 worldPos, float amplitudeMult, out float out_height, out Vector3 out_normal);

}