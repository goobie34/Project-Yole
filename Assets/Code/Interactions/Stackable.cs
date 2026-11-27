using UnityEngine;

public class Stackable : MonoBehaviour
{
    public Stackable above;
    public Stackable below;

    public Transform StackPoint;
    public bool isBeingCarried;

    public Interactable interactable;

    public void Start()
    {
        if (interactable == null) interactable = GetComponent<Interactable>(); // kräver att det finns en interactable på samma objekt
    }

    public void PickUp()
    {
        above = null;
        RemovedFromStack();

        isBeingCarried = true;
    }

    private void RemovedFromStack()
    {
        if (!HasBelow()) return;

        below.above = null;
        below.interactable.isCurrentlyInteractable = true;
        below = null;
    }

    public void PutDown(Stackable other)
    {
        above = null;
        Debug.Log("Test 1");
        if (!isBeingCarried) return;
        Debug.Log("Test 2");
        if (other != null) PlacedOnStack(other);
        
        isBeingCarried = false;
        Debug.Log("Test 5");
    }

    private void PlacedOnStack(Stackable other)
    {
        Debug.Log("Test 3");
        if (other == this || other.isBeingCarried || other.HasAbove()) return;
        Debug.Log("Test 4");
        below = other;
        other.above = this;
        other.interactable.isCurrentlyInteractable = false;
    }

    public bool HasAbove()
    {
        return above != null;
    }

    public bool HasBelow()
    {
        return below != null;
    }
}
