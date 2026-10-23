using UnityEngine;

public class SailingStateManager : MonoBehaviour
{
    public BoatStateMachine boatStateMachine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(boatStateMachine == null)
            boatStateMachine = GetComponent<BoatStateMachine>();


    }


    private void OnEventEnterState(BoatState newState)
    {
        if (newState != BoatState.Sailing_Control)
            return;

        EnableSailing();
    }

    private void OnEventExitState(BoatState oldState)
    {

    }

    private void EnableSailing()
    {

    }

    private void DisableSailing()
    {

    }
}
