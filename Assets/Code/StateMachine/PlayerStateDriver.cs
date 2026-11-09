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
    [Header("References")]
    [SerializeField] 
    //private NavCharController1 character;
    private NavCharController character;

    public PlayerContext ctx = new PlayerContext();

    string lastPath;
    StateMachine machine;
    State root;

    private void Awake()
    {
        if (character == null) character = GetComponent<NavCharController>();

        ctx.character = character;

        // Initialize StateMachine
        root = new PlayerRoot(null, ctx);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    private void Update()
    {
        if (machine == null) return;

        machine.Tick(Time.deltaTime);

        string path = StatePath(machine.Root.Leaf());

        if (path != lastPath)
        {
            Debug.Log("State" + path);
            lastPath = path;
        }
    }

    static string StatePath(State s)
    {
        return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
    }

    public void SetMovementInput(Vector2 input)
    {
        ctx.movementInput = Vector2.ClampMagnitude(input, 1f);
    }
}

[Serializable]
public class PlayerContext
{
    public NavCharController character;
    public Vector2 movementInput;
}
