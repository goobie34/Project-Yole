using UnityEngine;

public class SequencePlayer : MonoBehaviour
{
    private MaterialApplier materialApplier;
    public int stoneIndex; // TEMPORÄR, SKA TAS BORT!

    public void Awake()
    {
        materialApplier = gameObject.GetComponent<MaterialApplier>();
    }

    public void SequenceSelect()
    {
        materialApplier.ApplyYellow();
        Debug.Log("Stone index: " + stoneIndex);
    }

    public void SequenceDeselect()
    {
        materialApplier.ApplyDefault();
    }

    public bool CheckInput(int index)
    {
        return index == stoneIndex;
    }

    public void CorrectInput()
    {
        materialApplier.ApplyGreen();

        Debug.Log("Correct input for stone index: " + stoneIndex);
    }

    public void WrongInput()
    {
        materialApplier.ApplyRed();

        Debug.Log("Wrong input for stone index: " + stoneIndex);
    }
}
