using Unity.Cinemachine;
using UnityEngine;

public class CameraWavesAvoider : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow _camera;

    [SerializeField] private float offset = 0f;

    private Vector3 _defaultOffset;

   

    private void OnEnable()
    {
        if(_camera == null )
            _camera = GetComponent<CinemachineOrbitalFollow>();
        _defaultOffset = _camera.TargetOffset;
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

        _camera.TargetOffset = _defaultOffset + Vector3.up * ( height + offset);
    }
}
