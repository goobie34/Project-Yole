using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private ObjectSelector objectSelector;

    public bool canInteract = true;

    private void Start()
    {
        if (objectSelector == null)
        {
            objectSelector = gameObject.GetComponent<ObjectSelector>();
        }

        canInteract = true;
    }

    private void OnInteract(InputValue input)
    {
        if (!input.isPressed || objectSelector == null || !canInteract) return;

        IInteractable selectedInteractable = objectSelector.SelectedInteractable;

        if (selectedInteractable != null)
        {
            selectedInteractable.Interact();
        }
    }

    private void OnJump(InputValue input)
    {
        if (!input.isPressed || objectSelector == null || !canInteract) return;

        IInteractable selectedInteractable = objectSelector.SelectedInteractable;

        if (selectedInteractable is Interactable interactable)
        {
            if (interactable.TryGetComponent<Climbable>(out var climbable))
            {
                if (climbable != null)
                {
                    climbable.Climb();
                }
            }
        }
    }
}
