using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-100)] // detta är så att scriptet körs innan navcharcontroller och därmed innan spelaren studsar bort från en kant

public class EdgeDrop : MonoBehaviour
{
    [SerializeField] private float edgeCheckDistance = 0.15f;
    [SerializeField] private float dropCheckDistance = 1.2f;
    [SerializeField] private float minDropHeight = 0.35f;
    [SerializeField] private float maxDropHeight = 3.0f;
    [SerializeField] private float sampleDistance = 0.2f;

    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float minJumpDuration = 0.2f;
    [SerializeField] private float maxJumpDuration = 0.6f;
    [SerializeField] private float jumpHeight = 0.3f;

    private NavCharController controller;
    private bool isDropping;

    private void Awake()
    {
        controller = GetComponent<NavCharController>();
    }

    private void Update()
    {
        if (isDropping || !controller.enabled) return;

        Vector3 velocity = controller.CurrentVelocity;
        velocity.y = 0f;

        if (velocity.magnitude < 0.1f) return;

        if (!IsOnClimbable()) return;

        Vector3 direction = velocity.normalized;

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas)) return;

        float checkDistance = Mathf.Max(edgeCheckDistance, velocity.magnitude * Time.deltaTime);

        Vector3 nextPosition = hit.position + direction * checkDistance;

        if (!NavMesh.Raycast(hit.position, nextPosition, out NavMeshHit edgeHit, NavMesh.AllAreas)) return;

        if (TryGetLandingPosition(edgeHit.position, direction, out Vector3 target)) StartCoroutine(DropRoutine(target));
    }

    private bool IsOnClimbable()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.5f, objectLayer, QueryTriggerInteraction.Ignore)) return hit.collider.GetComponentInParent<Climbable>() != null;
        
        return false;
    }

    private bool TryGetLandingPosition(Vector3 edgePosition, Vector3 direction, out Vector3 target)
    {
        target = Vector3.zero;

        Vector3 origin = edgePosition + direction * dropCheckDistance + Vector3.up * 0.2f;

        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDropHeight + 0.2f, groundLayer, QueryTriggerInteraction.Ignore)) return false;

        float dropHeight = edgePosition.y - hit.point.y;

        if (dropHeight < minDropHeight || dropHeight > maxDropHeight) return false;

        if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, sampleDistance, NavMesh.AllAreas)) return false;

        float navDrop = edgePosition.y - navHit.position.y;

        if (navDrop < minDropHeight || navDrop > maxDropHeight) return false;

        target = navHit.position;

        return true;
    }

    private IEnumerator DropRoutine(Vector3 target)
    {
        isDropping = true;

        controller.enabled = false;
        controller.currentVelocity = Vector3.zero;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = target;

        float dropHeight = startPosition.y - endPosition.y;

        float heightFactor = Mathf.InverseLerp(minDropHeight, maxDropHeight, dropHeight);

        float jumpDuration = Mathf.Lerp(minJumpDuration, maxJumpDuration, heightFactor);

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            Vector3 position = Vector3.Lerp(startPosition, endPosition, t);

            position.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;
            
            transform.position = position;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        controller.enabled = true;
        controller.currentVelocity = Vector3.zero;

        isDropping = false;
    }
}

