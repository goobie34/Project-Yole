using UnityEngine;
using UnityEngine.AI;

public class PlayerCarryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform carryPoint;

    [Header("Drop Settings")]
    [SerializeField]
    private float dropDistance = 1.5f;
    [SerializeField]
    private float groundCheckHeight = 3f;
    [SerializeField]
    private float groundCheckDistance = 6f;
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
        if (pickupObject == null || IsCarrying || pickupObject.IsCarried || carryPoint == null) return false;

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

        carriedObject.transform.SetParent(carryPoint);
        carriedObject.transform.localPosition = Vector3.zero;
        carriedObject.transform.localRotation = Quaternion.identity;

        Debug.Log("Picked up: " + carriedObject.name);
        return true;
    }

    public bool TryDrop()
    {
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

        Debug.Log("Dropped: " + droppedObject.name);
        return true;
    }
}
