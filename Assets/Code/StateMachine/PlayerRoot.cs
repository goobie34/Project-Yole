using UnityEngine;

public class Carrying : State
{
    readonly PlayerContext ctx;

    public Carrying(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        Debug.Log("Entered Carrying");
    }

    protected override State GetTranstion()
    {
        if (ctx.dropPressed)
        {
            ctx.dropPressed = false;

            if (ctx.carryController.TryDrop()) return ((PlayerRoot)Parent).Grounded;
        }

        return !ctx.carryController.IsCarrying ? ((PlayerRoot)Parent).Grounded : null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.character.Move(ctx.movementInput);
    }
}

public class Move : State
{
    readonly PlayerContext ctx;

    public Move(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetTranstion()
    {
        return ctx.movementInput.sqrMagnitude <= 0.01f ? ((Grounded)Parent).Idle : null;
    }

    protected override void OnEnter()
    {
        Debug.Log("Entered Move");
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.character.Move(ctx.movementInput);
    }
}

public class Idle : State
{
    readonly PlayerContext ctx;

    public Idle(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetTranstion()
    {
        return ctx.movementInput.sqrMagnitude > 0.01f ? ((Grounded)Parent).Move : null;
    }

    protected override void OnEnter()
    {
        Debug.Log("Entered Idle");
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.character.Move(Vector2.zero);
    }
}

public class Grounded : State
{
    readonly PlayerContext ctx;

    public readonly Idle Idle;
    public readonly Move Move;

    public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
    }

    protected override State GetInitialState() => Idle;  
}

public class PlayerRoot : State
{
    public readonly Grounded Grounded;
    public readonly Carrying Carrying;
    readonly PlayerContext ctx;

    public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
        Carrying = new Carrying(m, this, ctx);
    }

    protected override State GetInitialState() => Grounded;

    protected override State GetTranstion()
    {
        if (ctx.carryController == null) return null;

        if (ctx.carryController.IsCarrying && ActiveChild == Grounded) return Carrying;

        return null;
    }
}
