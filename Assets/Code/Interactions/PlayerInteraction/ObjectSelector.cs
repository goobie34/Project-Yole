using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private float range = 3f;
    [SerializeField] private float coneAngle = 30f;

    private Vector3 searchDirection = Vector3.forward;
    private Vector3 targetDirection = Vector3.forward;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private float mouseSensitivity = 0.2f;
    private float mouseAngle = 0f;
    [SerializeField] private Transform pointerTransform;
    [SerializeField] private float returnDelay = 1f;
    private float lastLookTime = 0f;

    [SerializeField] private MeshRenderer pointer;
    [SerializeField] private float fadeSpeed = 5f;

    private MaterialPropertyBlock propertyBlock;
    private float currentAlpha = 0f;
    private float targetAlpha = 0f;
    private Color pointerColor;

    private IInteractable currentInteractable;

    public IInteractable SelectedInteractable {  get { return currentInteractable; } }

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        pointerColor = pointer.sharedMaterial.GetColor("_BaseColor");
        UpdatePointerMaterial();
    }

    private void UpdatePointerFade()
    {
        currentAlpha = Mathf.MoveTowards(
            currentAlpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        UpdatePointerMaterial();
    }

    private void UpdatePointerMaterial()
    {
        Color color = pointerColor;
        color.a *= currentAlpha;

        pointer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        pointer.SetPropertyBlock(propertyBlock);
    }

    private void Update()
    {
        IInteractable newInteractable = FindBestInteractable();

        if (ReferenceEquals(currentInteractable, newInteractable)) return;

        //if (!currentInteractable.IsCurrentlyInteractable) return;

        currentInteractable?.Deselect();

        currentInteractable = newInteractable;

        currentInteractable?.Select();
    }

    private void LateUpdate()
    {
        UpdateInteractionPointer();
    }

    private void OnGamePadLook(InputValue input)
    {
        Vector2 lookInput = input.Get<Vector2>();

        if (lookInput.sqrMagnitude < 0.1f) return;

        targetDirection = new Vector3(lookInput.x, 0f, lookInput.y).normalized;

        lastLookTime = Time.time;
        targetAlpha = 1f;
    }

    private void OnMouseLook(InputValue input)
    {
        if (!Mouse.current.leftButton.isPressed) return;

        Vector2 lookInput = input.Get<Vector2>();

        if (lookInput.sqrMagnitude < 0.1f) return;

        if (Time.time - lastLookTime >= returnDelay)
        {
            targetDirection = searchDirection;
        }

        Vector3 mouseMovement = new Vector3(lookInput.x, 0f, lookInput.y);

        float rotationAmount = Vector3.Cross(targetDirection, mouseMovement).y;

        targetDirection = Quaternion.Euler(0f, rotationAmount * mouseSensitivity, 0f) * targetDirection;

        targetDirection.Normalize();

        lastLookTime = Time.time;
        targetAlpha = 1f;
    }

    private void UpdateInteractionPointer()
    {
        Vector3 defaultDirection = transform.forward;
        defaultDirection.y = 0f;
        defaultDirection.Normalize();

        if (Time.time - lastLookTime >= returnDelay && !Mouse.current.leftButton.isPressed)
        {
            targetDirection = defaultDirection;
            targetAlpha = 0f;
            mouseAngle = 0f;
        }

        searchDirection = Vector3.Slerp(searchDirection, targetDirection, rotationSpeed * Time.deltaTime).normalized;

        pointerTransform.rotation = Quaternion.LookRotation(searchDirection, Vector3.up);

        UpdatePointerFade();
    }

    private IInteractable FindBestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        IInteractable bestInteractable = null;
        //float bestAngle = float.MaxValue;
        float bestScore = float.MaxValue;

        foreach (Collider c in hits)
        {
            IInteractable interactable = c.GetComponentInParent<IInteractable>();
            
            if (interactable == null) continue;

            if (interactable.IsCurrentlyInteractable == false) continue;

            Vector3 direction = c.ClosestPoint(transform.position) - transform.position;

            if (direction.sqrMagnitude < 0.001f) continue;


            float distance = direction.magnitude;

            float distanceFactor = Mathf.Clamp01(distance / range);

            float allowedAngle = Mathf.Lerp(120f, coneAngle, distanceFactor);

            float angle = Vector3.Angle(searchDirection, direction);

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
