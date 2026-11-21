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
    [SerializeField]
    private string groundTag = "Ground";

    [Header("Placement")]
    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField]
    private float placementPadding = 0.05f;

    //private PickupObject carriedObject;
    private Transform carriedObject;

    public bool IsCarrying => carriedObject != null;
    public bool IsDropping { get; private set; }
    //public PickupObject CarriedObject => carriedObject;
    public Transform CarriedObject => carriedObject;

    private bool CanStartPickup()
    {
        if (carryAnimation == null) carryAnimation = GetComponent<CarryAnimationController>();
        return isActiveAndEnabled && carryPoint != null && !IsCarrying && !IsDropping && (carryAnimation == null || !carryAnimation.IsAnimating);
    }

    public bool TryPickupTop(DockStackController stack)
    {
        if (!CanStartPickup() || stack == null || !stack.CanReach(transform.position)) return false;
        if (!stack.TryTakeTop(out ItemInstance top)) return false;

        BeginCarry(top.transform);
        return true;
    }

    public bool TryPickup(Component target)
    {
        if (target == null || !CanStartPickup()) return false;
        ItemInstance item = target.GetComponentInParent<ItemInstance>();
        Transform root = item != null ? item.transform : target.transform;

        if (root.GetComponentInParent<DockInventory>() != null || DockStackController.IsStored(root)) return false;
        WorldItemInteraction state = root.GetComponent<WorldItemInteraction>();

        if (state != null && (state.IsCarried || state.IsStored)) return false;
        BeginCarry(root);
        return true;
    }

    private void BeginCarry(Transform root)
    {
        carriedObject = root;
        root.SetParent(null, true);

        WorldItemInteraction world = WorldItemInteraction.GetOrCreate(root);
        world.SetStored(false);
        world.SetCarried(true);

        NavMeshObstacle obstacle = root.GetComponent<NavMeshObstacle>();
        if (obstacle != null) obstacle.enabled = false;

        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }
        if (carryAnimation != null && carryAnimation.CanPlayPickup)
            carryAnimation.PlayPickup(root);
        else
        {
            root.SetParent(carryPoint);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
        }
    }

    public bool TryDrop()
    {
        if (!IsCarrying || IsDropping || (carryAnimation != null && carryAnimation.IsAnimating)) return false;

        Vector3 dropPosition = transform.position + transform.forward * dropDistance;
        Vector3 rayOrigin = dropPosition + Vector3.up * groundCheckHeight;

        if (!TryFindGround(rayOrigin, out RaycastHit hit)) return false;

        Transform droppedObject = carriedObject;

        Collider objectCollider = droppedObject.GetComponentInChildren<Collider>();

        float bottomOffset = objectCollider != null ? droppedObject.position.y - objectCollider.bounds.min.y : 0f;
        Vector3 position = hit.point + Vector3.up * (bottomOffset + placementPadding);

        if (objectCollider != null)
        {
            Vector3 center = position + (objectCollider.bounds.center - droppedObject.position);

            Collider[] overlaps = Physics.OverlapBox(center, objectCollider.bounds.extents * 0.95f, Quaternion.identity, obstacleMask, QueryTriggerInteraction.Ignore);

            foreach (Collider overlap in overlaps)
            {
                if (overlap.transform == droppedObject || overlap.transform.IsChildOf(droppedObject)) continue;
                Debug.LogWarning("Cannot drop object: Placement blocked by " + overlap.name);
                return false;
            }
        }

        IsDropping = true;
        System.Action complete = () =>
        {
            if (droppedObject != null)
            {
                droppedObject.SetParent(null, true);
                droppedObject.SetPositionAndRotation(position, Quaternion.identity);

                WorldItemInteraction world = WorldItemInteraction.GetOrCreate(droppedObject);
                world.SetStored(false);
                world.SetCarried(false);
                PushPlayerOut(objectCollider);

                NavMeshObstacle obstacle = droppedObject.GetComponent<NavMeshObstacle>();
                if (obstacle != null) obstacle.enabled = true;

                Rigidbody rb = droppedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }
            carriedObject = null;
            IsDropping = false;
        };

        if (carryAnimation != null && carryAnimation.CanPlayDrop)
            carryAnimation.PlayDrop(droppedObject, position, complete);
        else complete();
        return true;
    }

    private bool TryFindGround(Vector3 rayOrigin, out RaycastHit groundHit)
    {
        groundHit = default;
        if (string.IsNullOrWhiteSpace(groundTag))
        {
            Debug.LogWarning("Cannot drop object: Set Ground Tag on PlayerCarryController.");
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, groundCheckDistance, ~0, QueryTriggerInteraction.Ignore);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit candidate in hits)
        {
            if (candidate.collider == null) continue;
            Transform surface = candidate.collider.transform;

            if (BelongsTo(surface, carriedObject) || BelongsTo(surface, transform) || (playerTransform != null && BelongsTo(surface, playerTransform))) continue;

            if (!candidate.collider.CompareTag(groundTag))
            {
                Debug.LogWarning("Cannot drop object: First surface '" + candidate.collider.name + "' must have the tag '" + groundTag + "'.");
                return false;
            }
            groundHit = candidate;
            return true;
        }

        Debug.LogWarning("Cannot drop object: No ground collider found within Ground check Distance");
        return false;
    }

    private static bool BelongsTo(Transform target, Transform root) => root != null && (target == root || target.IsChildOf(root));

    public bool TryReturnToStack(DockStackController stack)
    {
        if (!IsCarrying || IsDropping || stack == null || !stack.CanReach(transform.position) || (carryAnimation != null && carryAnimation.IsAnimating)) return false;

        ItemInstance item = carriedObject.GetComponent<ItemInstance>();
        if (item == null || !stack.TryPlace(item, out Vector3 position)) return false;
        IsDropping = true;
        System.Action complete = () =>
        {
            if (stack != null) stack.CompletePlacement(item, position);
            carriedObject = null;
            IsDropping = false;
        };
        if (carryAnimation != null && carryAnimation.CanPlayDrop)
            carryAnimation.PlayDrop(item.transform, position, complete);
        else complete();
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
