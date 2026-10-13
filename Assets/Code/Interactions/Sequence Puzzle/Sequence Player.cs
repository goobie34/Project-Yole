using UnityEngine;
using UnityEngine.Events;

public class SequencePlayer : MonoBehaviour
{
    //private MaterialApplier materialApplier;
    public int stoneIndex; // TEMPORÄR, SKA TAS BORT! Eller kanske inte, kanske bara behöver ett namnbyte(?)

    public UnityEvent OnCorrectInput = new UnityEvent();
    public UnityEvent OnWrongInput = new UnityEvent();
    public UnityEvent OnSequenceSelect = new UnityEvent();
    public UnityEvent OnSequenceDeselect = new UnityEvent();

    public void Awake()
    {
        //materialApplier = gameObject.GetComponent<MaterialApplier>();
    }

    public void SequenceSelect()
    {
        //materialApplier.ApplyYellow();
        OnSequenceSelect?.Invoke();
        Debug.Log("Stone index: " + stoneIndex);
    }

    public void SequenceDeselect()
    {
        //materialApplier.ApplyDefault();
        OnSequenceDeselect?.Invoke();
    }

    public bool CheckInput(int index)
    {
        return index == stoneIndex;
    }

    public void CorrectInput()
    {
        //materialApplier.ApplyGreen();
        OnCorrectInput?.Invoke();

        Debug.Log("Correct input for stone index: " + stoneIndex);
    }

    public void WrongInput()
    {
        //materialApplier.ApplyRed();
        OnWrongInput?.Invoke();

        Debug.Log("Wrong input for stone index: " + stoneIndex);
    }
}
