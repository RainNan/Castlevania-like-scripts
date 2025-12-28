using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idle;
    public Enemy_MoveState move;
    
    
    private readonly int XVelocityHash = Animator.StringToHash("x_velocity");

    protected override void Awake()
    {
        base.Awake();
        
        idle = new Enemy_IdleState(StateMachine, this);
        move = new Enemy_MoveState(StateMachine, this);
    }

    protected override void Update()
    {
        base.Update();
        
        anim.SetFloat(XVelocityHash, rb.velocity.x);
    }
}