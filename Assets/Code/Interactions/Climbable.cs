using System.Collections;
using UnityEngine;

public class Climbable : MonoBehaviour
{
    [SerializeField] private Transform climbPoint;
    [SerializeField] private float climbDuration = 0.5f;
    [SerializeField] private float jumpHeight = 0.5f;

    private bool isClimbing;

    public void Climb()
    {
        if (isClimbing) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("Player found: " + (player != null));
        Debug.Log("Found Player: " + player.name, player);

        if (player == null) return;

        player = player.transform.parent.gameObject; // la till detta för annars tog den ett child objekt vid namn "colider" som också hade player taggen, vet inte vad den gör

        StartCoroutine(ClimbRoutine(player));
    }

    private IEnumerator ClimbRoutine(GameObject player)
    {
        isClimbing = true;

        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null ) controller.enabled = false;

        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = climbPoint.position;

        float elapsedTime = 0f;

        while (elapsedTime < climbDuration)
        {
            float t = elapsedTime / climbDuration;

            Vector3 position = Vector3.Lerp(startPosition, endPosition, t);

            position.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            player.transform.position = position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPosition;

        if (controller != null) controller.enabled = true;

        isClimbing = false;
    }
}
