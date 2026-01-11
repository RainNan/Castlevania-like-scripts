using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = Vector2.zero;

        enemy.triggerAttack();
    }
}