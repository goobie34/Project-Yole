using UnityEngine;

public class GameStateLogger : MonoBehaviour
{
    public string testState;

    public bool enterStateBtn;

    private IGameStateService gameStateService;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void OnDisable()
    {
        if (gameStateService == null)
            return;


        gameStateService.GetStateChangeEvent().RemoveListener(LogStateChange);

        gameStateService = null;
    }

    private void TrySubscribeToStateChange()
    {
        if (gameStateService != null)
            return;

        if (GameStatesLocator.Instance == null)
            return;

        gameStateService = GameStatesLocator.Instance.Get();
        gameStateService.GetStateChangeEvent().AddListener(LogStateChange);
    }


    private void LogStateChange(GameStateChangeData data)
    {
        Debug.Log($"Game State Change: new state {data.nextState}, last state {data.previousState}");
    }

    private void Update()
    {
        TrySubscribeToStateChange();


        if (!enterStateBtn)
            return;

        enterStateBtn = false;

        if (!GameStatesLocator.Instance.TryGet(out var service))
            return;

        service.EnterState(testState);

    }

}
