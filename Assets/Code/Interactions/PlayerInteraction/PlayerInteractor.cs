using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private ObjectSelector objectSelector;

    private IInteractable selectedInteractable;

    private void Start()
    {
        if (objectSelector == null)
        {
            gameObject.GetComponent<ObjectSelector>();
        }
    }

    private void Update()
    {
        if (objectSelector != null)
        {
            selectedInteractable = objectSelector.SelectedInteractable;
        }
    }

    private void Interact()
    {
        // Behöver implementera något sätt att interagera med objektet som är selected. I min hjärna blir det väl genom ett knapptryck, typ F eller E.
    }
}
