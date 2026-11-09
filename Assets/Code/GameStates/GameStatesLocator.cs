using UnityEngine;
using UnityEngine.Events;

public class GameStatesLocator : ServiceLocator<IGameStateService>
{
    
}

public interface IGameStateService
{
    public UnityEvent<GameStateChangeData> GetStateChangeEvent();

    public void EnterState(string nextState);

    public string GetCurrentState();
}

public struct GameStateChangeData
{
    public string nextState;
    public string previousState;

}