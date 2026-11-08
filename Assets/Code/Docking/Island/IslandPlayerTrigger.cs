using UnityEngine;
using UnityEngine.Events;

public class IslandPlayerTrigger : MonoBehaviour
{
    public UnityEvent<GameObject> EVENT_OnPlayerEnter = new();
    public UnityEvent<GameObject> EVENT_OnPlayerExit = new();

    
    private void OnPlayerEnter(GameObject player)
    {
        Debug.Log("Player Enter Object");

        EVENT_OnPlayerEnter?.Invoke(player);

    }

    private void OnPlayerExit(GameObject player)
    {
       
        Debug.Log("Player Exit Object");


        EVENT_OnPlayerExit?.Invoke(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerIslandInstanceHandeler>();

        if (player == null)
            return;

        OnPlayerEnter(player.gameObject);

    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponentInParent<PlayerIslandInstanceHandeler>();

        if (player == null)
            return;

        OnPlayerExit(player.gameObject);
    }
}
