public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        stateMachine.ChangeState(enemy.move);
    } 
}