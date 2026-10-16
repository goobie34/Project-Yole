using UnityEngine;
using UnityEngine.Events;

public class BoatStateMachine : MonoBehaviour
{
    [SerializeField] private BoatState startingState = BoatState.Sailing_Control;
    [SerializeField] private bool testOverride = false;
    public BoatState currentState { get; private set; }

    public UnityEvent<BoatState> EVENT_OnStateEnter = new();
    public UnityEvent<BoatState> EVENT_OnStateExit = new();


    public void Start()
    {
        SetState(startingState);
    }

    private void OnDestroy()
    {
        EVENT_OnStateEnter.RemoveAllListeners();
        EVENT_OnStateExit.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
        if (testOverride)
        {
            if (startingState != currentState)
                SetState(startingState);
        }
    }

    public void SetState(BoatState state)
    {


        var oldState = currentState;
        
        currentState = state;

        OnStateChange(oldState, state);
    }

    private void OnStateChange(BoatState oldState, BoatState newState)
    {
        EVENT_OnStateExit.Invoke(oldState);
        EVENT_OnStateEnter.Invoke(newState);
    }

}

public enum BoatState
{
    Init            = 0,

    Sailing_Control = 10,
    Rowing_Control  = 11,

    Stunned_Sailing = 20,
    Stunned_Rowing  = 21,

    Docked          = 30,
    Docking         = 31,

    Death           = 40,
}