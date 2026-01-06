using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idle;
    public Enemy_MoveState move;
    public Enemy_AttackState attack;

    private readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private readonly int TriggerAttackHash = Animator.StringToHash("TriggerAttack");

    protected override void Awake()
    {
        base.Awake();

        idle = new Enemy_IdleState(StateMachine, this);
        move = new Enemy_MoveState(StateMachine, this);
        attack = new Enemy_AttackState(StateMachine, this);
    }

    protected override void Update()
    {
        base.Update();

        anim.SetFloat(XVelocityHash, rb.velocity.x);
    }

    public virtual void OnAttackEnd()
    {
        StateMachine.ChangeState(move);
    }

    public void triggerAttack()
    {
        anim.SetTrigger(TriggerAttackHash);
    }
}