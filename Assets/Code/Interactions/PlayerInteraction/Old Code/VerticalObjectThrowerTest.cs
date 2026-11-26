using System.Security;
using UnityEngine;

public class VerticalObjectThrowerTest : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private float verticalForce = 5f;

    private void Start()
    {
        if (interactable != null)
            interactable.OnInteract.AddListener(VerticalLauncher);
    }

    private void VerticalLauncher()
    {
        Rigidbody rb = interactable.GetComponent<Rigidbody>();
        Vector3 forceVector = new Vector3(0, verticalForce, 0);
        rb.AddForce(forceVector, ForceMode.Impulse);
    }
}
