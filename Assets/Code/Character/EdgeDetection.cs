using UnityEngine;

public class EdgeDetection : MonoBehaviour
{
    [Header("Edge Detection")]
    [SerializeField]
    private float edgeCheckDistance = 0.6f;
    [SerializeField]
    private float raycastHeight = 1f;
    [SerializeField]
    private float raycastLength = 2f;
    [SerializeField]
    private LayerMask groundLayer;

    public bool IsGroundAhead(Vector3 movement)
    {
        if (movement.sqrMagnitude < 0.0001f)
            return true;

        Vector3 direction = movement.normalized;

        Vector3 origin = transform.position + movement + direction * edgeCheckDistance + Vector3.up * raycastHeight;

        bool hitGround = Physics.Raycast(origin, Vector3.down, raycastLength, groundLayer);

        Debug.DrawRay(origin, Vector3.down * raycastLength, hitGround ? Color.green : Color.red);

        return hitGround;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + transform.forward * edgeCheckDistance + Vector3.up * raycastHeight;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(origin, origin + Vector3.down * raycastLength);
    }
}
