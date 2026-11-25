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
        if (!isBeingCarried) return;
        if (other != null) PlacedOnStack(other);

        isBeingCarried = false;
    }


    private void PlacedOnStack(Stackable other)
    {
        if (other == this || other.isBeingCarried || other.HasAbove()) return;

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
