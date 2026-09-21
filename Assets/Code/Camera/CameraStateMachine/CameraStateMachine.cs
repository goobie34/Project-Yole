using System.Collections;
using UnityEngine;

public class CameraStateMachine : MonoBehaviour
{
    private CameraState _currentState;
    private CameraState _nextState;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        CameraStateMachineLocator.Instance.Register(this);
    }

    private void OnDestroy()
    {
        CameraStateMachineLocator.Instance?.DeregisterIfThis(this);

    }

    private void Update()
    {
        if(_nextState != null)
            SetCurrentState(_nextState);
    }

    private void SetCurrentState(CameraState currentState)
    {
        _nextState = null;
        _currentState?.OnExitState();

        if(currentState == null)
        {
            Debug.LogWarning("Null Camera State Enterd");
        }

        _currentState = currentState;
        _currentState?.OnEnterState();
    }

    public void SetNextState(CameraState nextState)
    {
        _nextState = nextState;
    }

}

public interface ICameraState
{
    public void OnEnterState()
    {

    }

    public void OnExitState()
    {

    }
}
