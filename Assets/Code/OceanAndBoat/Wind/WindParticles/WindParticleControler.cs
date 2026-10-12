using UnityEngine;

public class WindParticleControler : MonoBehaviour
{
    [SerializeField] private ParticleSystemForceField _forceField;

    [SerializeField] private float _maxForce = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_forceField == null)
            _forceField = GetComponent<ParticleSystemForceField>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_forceField == null)
            return;

        if (WindServiceLocator.Instance == null)
            return;

        if (!WindServiceLocator.Instance.TryGet(out var windService))
            return;

        windService.EvaluateWind(transform.position, Time.time, out var windDirection, out var windMagnitude);

        var windForce = windDirection * Unity.Mathematics.math.smoothstep(0, _maxForce, windMagnitude) * _maxForce;
        
        UpdateParticleWind(windForce);
    }

    private void UpdateParticleWind(Vector3 wind)
    {
        
        _forceField.directionX = wind.x;
        _forceField.directionY = wind.y;
        _forceField.directionZ = wind.z;
    }
}
