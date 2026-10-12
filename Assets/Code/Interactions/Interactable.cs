using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    void Interact();
    void Select();
    void Deselect();
}

public class Interactable : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract = new UnityEvent();
    public UnityEvent OnSelect = new UnityEvent();
    public UnityEvent OnDeselect = new UnityEvent();

    public void Interact()
    {
        //Debug.Log("Interacted with " + gameObject.name);
        OnInteract?.Invoke();
    }

    public void Select()
    {
        //Debug.Log("Selected " + gameObject.name);
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        //Debug.Log("Deselected " + gameObject.name);
        OnDeselect?.Invoke();
    }
}
