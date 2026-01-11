using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        if (enemy.IsWallTouched)
        {
            enemy.Flip();
        }

        rb.velocity = new Vector2(enemy.IsFaceRight ? enemy.MoveSpeed : -enemy.MoveSpeed, rb.velocity.y);
    }
}