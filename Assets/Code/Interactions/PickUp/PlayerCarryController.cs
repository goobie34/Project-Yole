using UnityEngine;
using UnityEngine.AI;

public class PlayerCarryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform carryPoint;
    [SerializeField]
    private CarryAnimationController carryAnimation;

    [Header("Player Push")]
    [SerializeField]
    private Transform playerTransform;
    [SerializeField] 
    private float playerRadius = 0.4f;
    [SerializeField]
    private float pushPadding = 0.2f;
    [SerializeField]
    private float navMeshSearchRadius = 1f;


    [Header("Drop Settings")]
    [SerializeField]
    private float dropDistance = 1.5f;
    [SerializeField]
    private float groundCheckHeight = 3f;
    [SerializeField]
    private float groundCheckDistance = 6f;
    public bool IsDropping { get; private set; }
    [SerializeField]
    private LayerMask groundMask;

    [Header("Placement")]
    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField]
    private float placementPadding = 0.05f;

    private PickupObject carriedObject;

    public bool IsCarrying => carriedObject != null;
    public PickupObject CarriedObject => carriedObject;

    public bool TryPickup(PickupObject pickupObject)
    {
        if (carryAnimation == null) carryAnimation = GetComponent<CarryAnimationController>();
        if (pickupObject == null || IsCarrying || pickupObject.IsCarried || carryPoint == null) return false;
        if (carryAnimation != null && carryAnimation.IsAnimating) return false;

        carriedObject = pickupObject;
        carriedObject.SetCarried(true);

        NavMeshObstacle obstacle = carriedObject.GetComponent<NavMeshObstacle>();
        if (obstacle != null) obstacle.enabled = false;

        Rigidbody rb = carriedObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (carryAnimation != null) carryAnimation.PlayPickup(carriedObject.transform);
        else
        {
            carriedObject.transform.SetParent(carryPoint);
            carriedObject.transform.localPosition = Vector3.zero;
            carriedObject.transform.localRotation = Quaternion.identity;
        }    

        Debug.Log("Picked up: " + carriedObject.name);
        return true;
    }

    public bool TryDrop()
    {
        if (carryAnimation != null && carryAnimation.IsAnimating) return false;
        if (!IsCarrying) return false;

        Vector3 dropPosition = transform.position + transform.forward * dropDistance;
        Vector3 rayOrigin = dropPosition + Vector3.up * groundCheckHeight;

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            Debug.LogWarning("Cannot drop object: No ground found.");
            return false;
        }

        PickupObject droppedObject = carriedObject;
        Collider objectCollider = droppedObject.GetComponent<Collider>();

        float bottomOffset = 0f;

        if (objectCollider != null)
        {
            bottomOffset = droppedObject.transform.position.y - objectCollider.bounds.min.y;
        }

        Vector3 finalPosition = hit.point + Vector3.up * (bottomOffset + placementPadding);        

        if (objectCollider != null)
        {
            Vector3 halfExtents = objectCollider.bounds.extents * 0.95f;
            Vector3 checkCenter = finalPosition + (objectCollider.bounds.center - droppedObject.transform.position);

            Collider[] overlaps = Physics.OverlapBox(checkCenter, halfExtents, droppedObject.transform.rotation, obstacleMask, QueryTriggerInteraction.Ignore);

            foreach (Collider overlap in overlaps)
            {
                if (overlap.transform == droppedObject.transform || overlap.transform.IsChildOf(droppedObject.transform)) continue;

                Debug.LogWarning("Cannot drop object: Placement blocked by " + overlap.name);
                return false;
            }
        }

        if (carryAnimation != null)
        {
            IsDropping = true;

            carryAnimation.PlayDrop(droppedObject.transform, finalPosition, () =>
            {
                droppedObject.SetCarried(false);

                PushPlayerOut(objectCollider);

                NavMeshObstacle obstacle = droppedObject.GetComponent<NavMeshObstacle>();
                if (obstacle != null) obstacle.enabled = true;

                Rigidbody rb = droppedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;

                carriedObject = null;
                IsDropping = false;
            });
        }
        else
        {
            droppedObject.transform.SetParent(null);
            droppedObject.transform.position = finalPosition;
            droppedObject.SetCarried(false);

            NavMeshObstacle obstacle = droppedObject.GetComponent<NavMeshObstacle>();
            if (obstacle != null) obstacle.enabled = true;

            Rigidbody rb = droppedObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
            }

            carriedObject = null;          
        }
        Debug.Log("Dropped: " + droppedObject.name);
        return true;
    }

    private void PushPlayerOut(Collider boxCollider)
    {
        if (playerTransform == null || boxCollider == null) return;

        Bounds bounds = boxCollider.bounds;
        Vector3 playerPosition = playerTransform.position;

        Vector3 closest = bounds.ClosestPoint(playerPosition);
        Vector3 difference = playerPosition - closest;
        difference.y = 0f;

        if (difference.sqrMagnitude >= playerRadius * playerRadius) return;

        Vector3 direction = difference.normalized;

        if (direction.sqrMagnitude <= 0.001f)
        {
            direction = playerPosition - bounds.center;
            direction.y = 0f;
            direction = direction.sqrMagnitude >= 0.001f ? direction.normalized : -playerTransform.forward;
        }

        Vector3 targetPosition = playerPosition;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            targetPosition.x = bounds.center.x + Mathf.Sign(direction.x) * (bounds.extents.x + playerRadius + pushPadding);
        else 
            targetPosition.z = bounds.center.z + Mathf.Sign(direction.z) * (bounds.extents.z + playerRadius + pushPadding);

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            playerTransform.position = hit.position;
            Debug.Log("Player pushed out of dropped box.");
        }
    }
}
