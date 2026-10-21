using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private float range = 3f;
    [SerializeField] private float coneAngle = 30f;

    private IInteractable currentInteractable;

    public IInteractable SelectedInteractable {  get { return currentInteractable; } }

    private void Update()
    {
        IInteractable newInteractable = FindBestInteractable();

        if (ReferenceEquals(currentInteractable, newInteractable)) return;

        currentInteractable?.Deselect();

        currentInteractable = newInteractable;

        currentInteractable?.Select();
    }

    private IInteractable FindBestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        IInteractable bestInteractable = null;
        //float bestAngle = float.MaxValue;
        float bestScore = float.MaxValue;

        Vector3 forward = transform.forward;

        foreach (Collider c in hits)
        {
            IInteractable interactable = c.GetComponentInParent<IInteractable>();
            
            if (interactable == null) continue;

            Vector3 direction = c.ClosestPoint(transform.position) - transform.position;

            if (direction.sqrMagnitude < 0.001f) continue;


            float distance = direction.magnitude;

            float distanceFactor = Mathf.Clamp01(distance / range);

            float allowedAngle = Mathf.Lerp(120f, coneAngle, distanceFactor);

            float angle = Vector3.Angle(forward, direction);

            if (angle > allowedAngle) continue;

            float score = (angle / (coneAngle / 2f)) * 0.1f + (distance / range) * 0.9f;

            if (score < bestScore)
            {
                bestScore = score;
                bestInteractable = interactable;
            }
        }

        return bestInteractable;
    }

    private void OnDisable()
    {
        currentInteractable?.Deselect();
        currentInteractable = null;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Quaternion leftRotation = Quaternion.Euler(
            0, -coneAngle / 2f, 0
        );

        Quaternion rightRotation = Quaternion.Euler(
            0, coneAngle / 2f, 0
        );

        Gizmos.DrawRay(
            transform.position,
            leftRotation * forward * range
        );

        Gizmos.DrawRay(
            transform.position,
            rightRotation * forward * range
        );
    }
}
