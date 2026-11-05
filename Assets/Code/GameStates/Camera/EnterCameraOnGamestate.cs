using Unity.Cinemachine;
using UnityEngine;

public class EnterCameraOnGamestate : MonoBehaviour
{
    public string targetGameState;

    [Header("Dependencies")]
    public CinemachineCamera cameraState;

    private IGameStateService gameStateService;

    private void Start()
    {
        if(cameraState == null)
            cameraState = GetComponent<CinemachineCamera>();


       
    }

    void OnDisable()
    {
        if (gameStateService == null)
            return;


        gameStateService.GetStateChangeEvent().RemoveListener(OnEvent_ChangeGameState);

        gameStateService = null;
    }

    private void FixedUpdate()
    {
        TrySubscribeToStateChange();
    }

    private void TrySubscribeToStateChange()
    {
        if (gameStateService != null)
            return;

        if (GameStatesLocator.Instance == null)
            return;

        gameStateService = GameStatesLocator.Instance.Get();
        gameStateService.GetStateChangeEvent().AddListener(OnEvent_ChangeGameState);

        if (gameStateService.GetCurrentState() == targetGameState)
            EnterCameraState();
    }

    private void OnEvent_ChangeGameState(GameStateChangeData data)
    {
        if (data.nextState == targetGameState)
        {
            return;

        }
            

        EnterCameraState();


    }

    private void EnterCameraState()
    {
        if (cameraState == null)
            return;

        cameraState.Prioritize();
        
    }

   
}
