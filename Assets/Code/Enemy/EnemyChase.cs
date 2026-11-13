using UnityEngine;
using UnityEngine.Rendering;

public class EnemyChase : MonoBehaviour
{
    public GameObject player;
    private bool playerDetected = false;
    private bool isChasing = false;
    private float lostPlayerTimer = 0f;

    [SerializeField] private float speed;
    [SerializeField] private float stopChaseDelay;

    void Update()
    {
        if (isChasing)
        {
            // Count time while the player is outside the radius
            if (!playerDetected)
            {
                lostPlayerTimer += Time.deltaTime;

                // Stop chasing when the timer runs out
                if (lostPlayerTimer >= stopChaseDelay)
                {
                    isChasing = false;
                    return;
                }

            }

            // Chase the player
            transform.LookAt(player.transform.position);
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = true;
            isChasing = true;
            lostPlayerTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            lostPlayerTimer = 0f;
        }
    }
}
