using Unity.Cinemachine;
using UnityEngine;

public class CameraState : MonoBehaviour, ICameraState
{
    public bool TestBtn = false;


    [SerializeField] private CinemachineCamera _stateCamera;

    public bool activeState { get; private set; }

    public void LateUpdate()
    {
        if (TestBtn)
        {
            TestBtn = false;

           

            

            if(!CameraStateMachineLocator.Instance.TryGet(out var camera))
            {
                TestBtn = true;
                return;
            }
            
            camera.SetNextState(this);
            
        }
    }

    public virtual void OnEnterState()
    {
        if(_stateCamera == null)
            _stateCamera = GetComponentInChildren<CinemachineCamera>();

        _stateCamera.Prioritize();
    }

    public virtual void OnExitState()
    {

    }

    
}
