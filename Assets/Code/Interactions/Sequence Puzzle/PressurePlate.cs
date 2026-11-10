using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Interactable interactable;

    private bool isStoodOn = false;

    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float moveSpeed = 1f;

    private Vector3 startPosition;
    private Vector3 pressedPosition;

    private bool hasInteracted = false;

    void Start()
    {
        startPosition = transform.localPosition;

        pressedPosition = startPosition + Vector3.down * moveDistance;
    }

    private void Update()
    {
        Vector3 targetPosition;

        if (isStoodOn)
            targetPosition = pressedPosition;
        else
            targetPosition = startPosition;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

        if (isStoodOn && transform.localPosition == pressedPosition && !hasInteracted)
        {
            interactable?.Interact();
            hasInteracted = true;
        }

        if (!isStoodOn)
        {
            hasInteracted = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStoodOn = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStoodOn = false;
        }
    }
}