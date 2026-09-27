using Unity.Cinemachine;
using UnityEngine;

public class CameraWavesAvoider : MonoBehaviour
{

    [SerializeField] private CinemachineOrbitalFollow _camera;
    [SerializeField] private CinemachineHardLookAt _hardLookAt;

    [SerializeField] private float offset = 0f;
    [SerializeField] private float lookAtOffset;
    [SerializeField] private float lookAtLerp = 0.1f;
    [SerializeField] private float lookAtMult = 1f;
    private Vector3 _defaultOffset;

    private Vector3 _defaultLookAt;

    private float _targetLookAtOffset;
    private float _currentLookAtOffset;


    private void OnEnable()
    {
        if(_camera == null )
            _camera = GetComponent<CinemachineOrbitalFollow>();

        if(_hardLookAt == null )
            _hardLookAt = GetComponent<CinemachineHardLookAt>();

        _defaultOffset = _camera.TargetOffset;

        _defaultLookAt = _hardLookAt.LookAtOffset;
    }

   

    private Vector2 GetWorldPos()
    {
        var pos = _camera.transform.position;

        return new Vector2 (pos.x, pos.z);
    }


    

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
            return;

        var waterServcie = WaveServiceLocator.Instance?.Get();

        if(waterServcie == null)
            return;

        waterServcie.EvaluateWaves(GetWorldPos(), 1, out var height, out var normal);
        
        
        _camera.TargetOffset = _defaultOffset + Vector3.up * (height + offset);

        _targetLookAtOffset = (height + lookAtOffset) * lookAtMult;

        _currentLookAtOffset = Mathf.Lerp(_currentLookAtOffset, _targetLookAtOffset, lookAtLerp);

        _hardLookAt.LookAtOffset = _defaultLookAt + Vector3.up * _currentLookAtOffset;
    }
}
