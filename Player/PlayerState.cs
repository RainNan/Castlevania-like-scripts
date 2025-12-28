public abstract class PlayerState : EntityState
{
    protected readonly Player player;
    protected PlayerInputSet input;

    protected PlayerState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        base.Enter();
        player.currentStateName = GetType().Name;
    }
}