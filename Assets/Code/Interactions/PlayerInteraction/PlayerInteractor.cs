using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private ObjectSelector objectSelector;

    private void Start()
    {
        if (objectSelector == null)
        {
            objectSelector = gameObject.GetComponent<ObjectSelector>();
        }
    }

    private void OnInteract(InputValue input)
    {
        if (!input.isPressed || objectSelector == null) return;

        IInteractable selectedInteractable = objectSelector.SelectedInteractable;

        if (selectedInteractable != null)
        {
            selectedInteractable.Interact();
        }
    }
}
