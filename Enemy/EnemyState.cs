public class EnemyState : EntityState
{
    protected readonly Enemy enemy;

    public EnemyState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.currentStateName = GetType().Name;
    }
}