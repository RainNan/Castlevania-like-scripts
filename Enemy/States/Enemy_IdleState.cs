public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        stateMachine.ChangeState(enemy.Move);
    } 
}