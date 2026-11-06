using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameStatesManager : MonoBehaviour, IGameStateService
{
    private UnityEvent<GameStateChangeData> EVENT_StateChangeStart;
    
    private string CurrentState = "INIT_STATE";

    private IEnumerator Start()
    {
        while (this.isActiveAndEnabled)
        {
            GameStatesLocator.Instance?.Register(this);
            if (GameStatesLocator.Instance != null)
                break;
            yield return new WaitForFixedUpdate();
        }

        EVENT_StateChangeStart = new UnityEvent<GameStateChangeData>();
    }

    public UnityEvent<GameStateChangeData> GetStateChangeEvent()
    {
        return EVENT_StateChangeStart;
    }
    
    public string GetCurrentState() => CurrentState;

    public void EnterState(string nextState)
    {
        if (CurrentState == nextState)
            return;

        var previousState = CurrentState;
        CurrentState = nextState;

        TriggerStateChange(CurrentState, previousState);
    }

    private void TriggerStateChange(string nextState, string previousState)
    {
        var data = new GameStateChangeData()
        {
            nextState = nextState,
            previousState = previousState
        };

        EVENT_StateChangeStart?.Invoke(data);
    }
}
