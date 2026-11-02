using UnityEngine;

/// <summary>
/// Whole of the 'Move' class is just an example of an state but is not a state the player has, thus it needs to be updated accordingly to what how the player actually works
/// </summary>

//public class Move : State
//{
//    readonly PlayerContext ctx;

//    public Move(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
//    {
//        this.ctx = ctx;
//    }

//    protected override State GetTranstion()
//    {
//        if (!ctx.grounded) return ((PlayerRoot)Parent).Airborne;

//        return Mathf.Abs(ctx.move.x) <= 0.01f ? ((Grounded)Parent).Idle : null;
//    }

//    protected override void OnUpdate(float deltaTime)
//    {
//        var target = ctx.move.x * ctx.moveSpeed;
//        ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, target, ctx.accel * deltaTime);
//    }
//}

/// <summary>
/// Whole of the 'Idle' class is just an example of an state but is not a state the player has, thus it needs to be updated accordingly to what how the player actually works
/// </summary>

//public class Idle : State
//{
//    readonly PlayerContext ctx;

//    public Idle(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
//    {
//        this.ctx = ctx;
//    }

//    protected override State GetTranstion()
//    {
//        return Mathf.Abs(ctx.move.x) > 0.01f ? ((Grouned)Parent).Move : null;
//    }

//    protected override void OnEnter()
//    {
//        ctx.velocity.x = 0f;
//    }
//}

/// <summary>
/// Whole of the 'Grounded' class is just an example of an state but is not a state the player has, thus it needs to be updated accordingly to what how the player actually works
/// </summary>

//public class Grounded : State
//{
//    readonly PlayerContext ctx;
//    public readonly Idle Idle;
//    public readonly Move Move;

//    public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
//    {
//        this.ctx = ctx;
//        Idle = new Idle(m, this, ctx);
//        Move = new Move(m, this, ctx);
//        Add(new DelayActivationActivity { seconds = 0.50f });
//    }

//    protected override State GetInitialState() => Idle;

//    protected override State GetTranstion()
//    {
//        if (ctx.jumpPressed)
//        {
//            ctx.jumpPressed = false;
//            var rb = ctx.rb;
//            if (rb != null)
//            {
//                var v = rb.linearVelocity;
//                v.y = ctx.jumpSpeed;
//                rb.linearVelocity = v;
//            }
//            return ((PlayerRoot)Parent).Airborne;
//        }
//        return ctx.grounded ? null : ((PlayerRoot)Parent).Airborne;
//    }
//}

/// <summary>
/// Whole of the 'Airborne' class is just an example of an state but is not a state the player has, thus it needs to be updated accordingly to what how the player actually works
/// </summary>

//public class Airborne : State
//{
//    readonly PlayerContext ctx;

//    public Airborne(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
//    {
//        this.ctx = ctx;
//    }

//    protected override State GetTranstion() => ctx.grounded ? ((PlayerRoot)Parent).Grounded : null;

//    protected override void OnEnter()
//    {
//        // TODO : Update Animator through ctx.anim
//    }
//}

public class PlayerRoot : State
{
    //public readonly Grounded grounded; <-- Yet again the player does not use rb but instead a navmesh and needs to be updated accordingly
    readonly PlayerContext ctx;

    public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        //Grounded = new grounded(m, this, ctx); <-- Yet again the player does not use rb but instead a navmesh and needs to be updated accordingly
    }

    // protected override State GetInitialState() => Grounded; <-- Yet again the player does not use rb but instead a navmesh and needs to be updated accordingly
    // protected override State GetTransition() => ctx.grounded ? null : Airborne; <-- example of state transition but needs to be updated accordingly to the actual player controller


}
