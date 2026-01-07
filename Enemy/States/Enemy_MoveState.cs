using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    private float _elapsed = 0f;

    public Enemy_MoveState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!enemy.IsGrounded)
        {
            enemy.Flip();
        }
                
        _elapsed += Time.deltaTime;
        if (_elapsed >= stateTimer)
        {
            stateMachine.ChangeState(enemy.attack);
            _elapsed = 0;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        rb.velocity = new Vector2(enemy.IsFaceRight ? enemy.MoveSpeed : -enemy.MoveSpeed, rb.velocity.y);
    }
}