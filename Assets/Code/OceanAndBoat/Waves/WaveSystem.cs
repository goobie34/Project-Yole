using UnityEngine;

public class WaveSystem : MonoBehaviour, IWaveService
{
    [SerializeField] private SumOfSinesManager sumOfSinesManager;

    float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        WaveServiceLocator.Instance?.Register(this);
    }

    void OnDisable()
    {
        WaveServiceLocator.Instance?.DeregisterIfThis(this);
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
    }

    public void EvaluateWaves(Vector2 worldPos, float amplitudeMult, out float out_height, out Vector3 out_normal)
    {
        sumOfSinesManager.EvaluateWaves(worldPos, time, amplitudeMult, out out_height, out out_normal);
    }


}

[System.Serializable]
public class SumOfSinesManager
{
    public Wave[] _sines;

    public void EvaluateWaves(Vector2 worldPos, float time, float amplitudeMult, out float height, out Vector3 normal)
    {
        height = 0;
        var derivativeSum = Vector2.zero;

        float sine = 0;
        float derivative = 0;
        for (int i = 0; i < _sines.Length; i++)
        {
            _sines[i].Evaluate(worldPos, time, amplitudeMult, out sine, out derivative);

            height += sine;

            derivativeSum += derivative * _sines[i].direction.normalized;
        }


        normal = new Vector3(-derivativeSum.x, 1, -derivativeSum.y).normalized;

    }


    [System.Serializable]
    public struct Wave
    {
        public Vector2 direction;
        public float amplitude;
        public float wavelength;
        public float speed;
        public float steepness;

        public void Evaluate(Vector2 worldPos, float time, float amplitudeMult, out float sine, out float derivative)
        {

            float x = Vector2.Dot(worldPos, direction.normalized);

            float frequency = 2 / wavelength;

            float func = frequency * x + speed * frequency * time;

            float sineResult = (Mathf.Sin(func) + 1) * 0.5f;

            sine = 2 * amplitude * amplitudeMult * Mathf.Pow(sineResult, steepness);

            float h = Mathf.Pow(sineResult, Mathf.Max(1, steepness - 1));

            derivative = steepness * frequency * amplitude * amplitudeMult * h * Mathf.Cos(func);
        }
    }

}
