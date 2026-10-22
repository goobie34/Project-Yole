using UnityEngine;

public class CharacterTilt : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NavCharController character;
    [SerializeField]
    private Transform visualTransform;

    [Header("Tilt Settings")]
    [SerializeField]
    private float forwardTilt = 8f;
    [SerializeField]
    private float sidewaysTilt = 12f;
    [SerializeField]
    private float tiltSmoothness = 8f;
    [SerializeField]
    private float movementThreshold = 0.1f;
    [SerializeField]
    private float maxMovementSpeed = 10f;
    [SerializeField]
    private float turnSensitivity = 0.03f;

    [Header("Model Orientation")]
    [SerializeField]
    private Vector3 forwardTiltAxis = Vector3.right;
    [SerializeField]
    private Vector3 sidewaysTiltAxis = Vector3.forward;

    private Quaternion initialLocalRotation;
    private Vector3 previousDirection;

    private void Awake()
    {
        if (character == null)
            character = GetComponent<NavCharController>();

        if (visualTransform == null)
            visualTransform = transform;

        initialLocalRotation = visualTransform.localRotation;
    }

    private void LateUpdate()
    {
        if (character == null || visualTransform == null)
            return;

        Vector3 velocity = character.CurrentVelocity;
        velocity.y = 0f;

        float targetPitch = 0f;
        float targetRoll = 0f;

        if (velocity.magnitude > movementThreshold)
        {
            float speedFactor = Mathf.Clamp01(velocity.magnitude / Mathf.Max(0.01f, maxMovementSpeed));

            targetPitch = forwardTilt * speedFactor;

            Vector3 currentDirection = velocity.normalized;

            if (previousDirection.sqrMagnitude > 0.001f)
            {
                float turnAngle = Vector3.SignedAngle(previousDirection, currentDirection, Vector3.up);

                targetRoll = Mathf.Clamp(-turnAngle * turnSensitivity * sidewaysTilt, -sidewaysTilt, sidewaysTilt * speedFactor);
            }

            previousDirection = currentDirection;
        }
        else
        {
            previousDirection = Vector3.zero;
        }
        
        Quaternion forwardRotation = Quaternion.AngleAxis(targetPitch, forwardTiltAxis.normalized);

        Quaternion sidewaysRotation = Quaternion.AngleAxis(targetRoll, sidewaysTiltAxis.normalized);

        Quaternion targetRotation = forwardRotation * sidewaysRotation * initialLocalRotation;

        visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, targetRotation, 1f - Mathf.Exp(-tiltSmoothness * Time.deltaTime));
    }
}
