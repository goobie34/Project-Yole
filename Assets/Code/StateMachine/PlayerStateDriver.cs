using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerStateDriver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private NavCharController character;
    [SerializeField]
    private PlayerCarryController carryController;

    public PlayerContext ctx = new PlayerContext();
    private InputAction dropAction;

    string lastPath;
    StateMachine machine;
    State root;

    private void Awake()
    {
        if (character == null) character = GetComponent<NavCharController>();

        ctx.character = character;

        if (carryController == null) carryController = GetComponent<PlayerCarryController>();

        ctx.carryController = carryController;

        dropAction = InputSystem.actions?.FindAction("Player/Drop");

        if (dropAction == null) Debug.LogError("Drop Input Action not found!");

        // Initialize StateMachine
        root = new PlayerRoot(null, ctx);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    private void OnEnable()
    {
        dropAction?.Enable();
    }

    private void OnDisable()
    {
        dropAction?.Disable();
    }

    private void Update()
    {
        if (dropAction != null && dropAction.WasPressedThisFrame()) ctx.dropPressed = true;

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

    public PlayerCarryController carryController;
    public bool dropPressed;
}
