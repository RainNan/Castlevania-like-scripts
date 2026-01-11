using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    public Enemy_BattleState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.setIsBattle(true);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.setIsBattle(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        var playerCollider = enemy.raycastHit2D.collider;
        if (playerCollider is not null)
        {
            if (Mathf.Abs(enemy.transform.position.x - playerCollider.transform.position.x) <= enemy.attackRange)
                stateMachine.ChangeState(enemy.Attack);
        }
        else
            stateMachine.ChangeState(enemy.Move);
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        rb.velocity = new Vector2(
            enemy.battleMoveSpeed * enemy.GetFaceRightSign, rb.velocity.y);
    }
}