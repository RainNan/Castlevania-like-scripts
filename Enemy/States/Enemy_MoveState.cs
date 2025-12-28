using UnityEngine;

public class Enemy_MoveState : EnemyState
{
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
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        rb.velocity = new Vector2(enemy.IsFaceRight? enemy.MoveSpeed : -enemy.MoveSpeed, rb.velocity.y);
    }
}