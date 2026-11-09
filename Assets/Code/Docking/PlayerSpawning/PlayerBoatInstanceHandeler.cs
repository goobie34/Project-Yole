using UnityEngine;

public class PlayerBoatInstanceHandeler : MonoBehaviour
{
    private bool registerd;

    public GameObject player;
    private void Start()
    {
        TryRegister();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        TryRegister();


    }

    private void TryRegister()
    {
        if (registerd)
            return;

        if (PlayerManagerServiceLocator.Instance == null)
            return;

        if (!PlayerManagerServiceLocator.Instance.TryGet(out var service)) return;

        registerd = true;

        service.SetBoatPlayer(player);
    }
}
