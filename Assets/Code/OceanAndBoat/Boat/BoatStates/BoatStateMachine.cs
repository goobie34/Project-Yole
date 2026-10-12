using UnityEngine;

public class BoatStateMachine : MonoBehaviour
{
    [SerializeField] private BoatState startingState = BoatState.Sailing_Control;

    public BoatState currentState { get; private set; }

    public void Start()
    {
        SetState(startingState);
    }

    public void SetState(BoatState state)
    {
        var oldState = currentState;
        
        currentState = state;

        OnStateChange(oldState, state);
    }

    private void OnStateChange(BoatState oldState, BoatState newState)
    {

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