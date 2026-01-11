public class Enemy_GroundedState:EnemyState
{
    public Enemy_GroundedState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (!enemy.IsGrounded)
            enemy.Flip();
        
        if (enemy.raycastHit2D.collider is not null)
            stateMachine.ChangeState(enemy.Battle);
    }
}
