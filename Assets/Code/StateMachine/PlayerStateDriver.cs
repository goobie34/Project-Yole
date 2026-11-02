using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

public class PlayerStateDriver : MonoBehaviour
{
    public PlayerContext ctx = new PlayerContext();
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundMask;
    public bool drawGizmos = true;
    string lastPath;

    Rigidbody rb;
    StateMachine machine;
    State root;

    private void Awake()
    {
        //Video wants "rb = gameObject.GetOrAdd<rigidbody>(); -> character uses a navmesh and no rigidbody and this part needs to be updated accordingly

        //Video wants to constrain the ridigbody and share the rb to shared context rb but character uses navmesh not rb

        // ctx.anim = GetComponentInChildren<Animator>()

        // Initialize StateMachine
        root = new PlayerRoot(null, ctx);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    private void Update()
    {
        float x = 0f;
        // Left and right input to be put here
        // ctx.move.x = Mathf.Clamp(x, -1f, 1f); <-- To be adapted to actual movement logic in the game
        // Video says to add a physics.checksphere but since the game uses navmesh as a the movement logic and to check onground, etc. This section needs to be updated accordingly

        machine.Tick(Time.deltaTime);

        var path = StatePath(machine.Root.Leaf());

        if (path != lastPath)
        {
            Console.WriteLine("State", path);
            lastPath = path;
        }
    }

    private void FixedUpdate()
    {
        //Reads physics velocity of the rigidbody, but player character does not use rigidbody but instead navmesh
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || groundCheck == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    static string StatePath(State s)
    {
        return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
    }
}

[Serializable]
public class PlayerContext
{
    public float movementSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float sampleDistance = 0.3f;
    public float maxHeightDifference = 0.3f;
    public float edgePushSpeed = 2f;
    public float edgePushDuration = 0.15f;
    public Vector3 edgePushDirection;
    public float edgePushTimer;
    public Vector2 movementInput;
    public Vector3 currentVelocity;
    public Transform cameraTransform;
}
