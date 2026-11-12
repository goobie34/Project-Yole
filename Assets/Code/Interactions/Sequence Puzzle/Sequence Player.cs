using UnityEngine;
using UnityEngine.Events;

public class SequencePlayer : MonoBehaviour
{
    public int stoneIndex;

    public UnityEvent OnCorrectInput = new UnityEvent();
    public UnityEvent OnWrongInput = new UnityEvent();
    public UnityEvent OnSequenceSelect = new UnityEvent();
    public UnityEvent OnSequenceDeselect = new UnityEvent();

    public void SequenceSelect()
    {
        OnSequenceSelect?.Invoke();
        //Debug.Log("Stone index: " + stoneIndex);
    }

    public void SequenceDeselect()
    {
        OnSequenceDeselect?.Invoke();
    }

    public bool CheckInput(int index)
    {
        return index == stoneIndex;
    }

    public void CorrectInput()
    {
        OnCorrectInput?.Invoke();

        //Debug.Log("Correct input for stone index: " + stoneIndex);
    }

    public void WrongInput()
    {
        OnWrongInput?.Invoke();

        //Debug.Log("Wrong input for stone index: " + stoneIndex);
    }
}
