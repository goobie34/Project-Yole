using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NavMovement2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector2 inputV;

    public void Start()
    {
        if(agent == null)
            agent = GetComponent<NavMeshAgent>();

        
    }

    private void OnMove(InputValue input)
    {
        inputV = input.Get<Vector2>();

        
    }

  
}
