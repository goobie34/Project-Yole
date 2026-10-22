using UnityEngine;

public class MaterialApplier : MonoBehaviour
{
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material greenMaterial;

    [SerializeField] private SequencePlayer sequencePlayer;
    [SerializeField] private Interactable interactable;

    private void Start()
    {
        if (interactable != null)
        {
            interactable.OnSelect.AddListener(ApplyYellow);
            interactable.OnDeselect.AddListener(ApplyDefault);
        }

        if (sequencePlayer != null)
        {
            sequencePlayer.OnSequenceDeselect.AddListener(ApplyDefault);
            sequencePlayer.OnSequenceSelect.AddListener(ApplyYellow);
            sequencePlayer.OnCorrectInput.AddListener(ApplyGreen);
            sequencePlayer.OnWrongInput.AddListener(ApplyRed);
        }
        else Debug.Log("SequencePlayer was not found!");
    }

    public void SubscribeToSequencePlayer(SequencePlayer SP)
    {
        SP.OnSequenceDeselect.AddListener(ApplyDefault);
        SP.OnSequenceSelect.AddListener(ApplyYellow);
        SP.OnCorrectInput.AddListener(ApplyGreen);
        SP.OnWrongInput.AddListener(ApplyRed);
    }

    public void ApplyGreen()
    {
        GetComponent<Renderer>().material = greenMaterial;
    }

    public void ApplyYellow()
    {
        GetComponent<Renderer>().material = yellowMaterial;
    }

    public void ApplyRed()
    {
        GetComponent<Renderer>().material = redMaterial;
    }

    public void ApplyDefault()
    {
        GetComponent<Renderer>().material = originalMaterial;
    }

    private void OnDisable()
    {
        interactable?.Deselect();
        interactable = null;
    }
}
