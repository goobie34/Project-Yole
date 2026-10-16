using UnityEngine;

[RequireComponent (typeof(NavCharController))]
public class NavCharacterRotation : MonoBehaviour
{    
    private NavCharController character;

    [Header("Rotation")]
    [SerializeField]
    private float rotationSpeed = 10f;

    [Header("Debug Arrow")]
    [SerializeField]
    private bool showDirectionArrow = true;
    [SerializeField]
    private float arrowLength = 1.5f;
    [SerializeField]
    private float arrowHeight = 0.5f;
    [SerializeField]
    private Color arrowColor = Color.green;

    private void Awake()
    {
        character = GetComponent<NavCharController>();
    }

    private void LateUpdate()
    {
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        Vector3 direction = character.CurrentVelocity;

        direction.y = 0f;

        //Do not rotate when standing still
        if (direction.sqrMagnitude < 0.001f)
            return;

        //Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!showDirectionArrow)
            return;

        //Start position
        Vector3 start = transform.position + Vector3.up * arrowHeight;

        //Forward direction
        Vector3 direction = transform.forward;

        //End direction
        Vector3 end = start + direction * arrowLength;

        Gizmos.color = arrowColor;

        //Main arrow line
        Gizmos.DrawLine(start, end);

        //Arrow head
        Vector3 right = Quaternion.Euler(0,150, 0) * direction;
        Vector3 left = Quaternion.Euler(0, -150, 0) * direction;

        Gizmos.DrawLine(end, end + right * 0.3f);
        Gizmos.DrawLine(end, end + left * 0.3f);
    }
}
