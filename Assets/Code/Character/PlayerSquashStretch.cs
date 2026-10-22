using UnityEngine;

public class PlayerSquashStretch : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private NavCharController character;
    [SerializeField]
    private SquashStrech squashStretch;

    [Header("Movement Settings")]
    [SerializeField] 
    private float movementThreshold = 0.1f;
    [SerializeField] 
    private float animationInterval = 0.3f;

    private float animationTimer;

    private void Awake()
    {
        if (character == null)
            character = GetComponent<NavCharController>();

        if(squashStretch == null)
            squashStretch = GetComponent<SquashStrech>();
    }

    private void Update()
    {
        if (character == null || squashStretch == null)
            return;

        bool isMoving = character.CurrentVelocity.sqrMagnitude > movementThreshold * movementThreshold;

        if (isMoving)
        {
            animationTimer -= Time.deltaTime;

            if (animationTimer <= 0)
            {
                squashStretch.PlaySquashAndStretch();

                animationTimer = animationInterval;
            }
        }
        else
        {
            animationTimer = 0f;
        }
    }
}
