
public class Player_JumpState : PlayerState
{
    public Player_JumpState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        var v = player.rb.velocity;
        v.y = player.JumpForce;
        player.rb.velocity = v;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
               
        // => Dash
        if (player.DashPressed)
        {
            stateMachine.ChangeState(player.Dash);
            return;
        }

        if (player.IsWallTouched)
        {
            stateMachine.ChangeState(player.Slide);
            return;
        }
        
        // 上升结束 -> Fall
        if (player.rb.velocity.y <= 0f)
        {
            stateMachine.ChangeState(player.Fall);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        // 空中也允许水平控制
        var v = player.rb.velocity;
        v.x = player.MoveInput.x * player.MoveSpeed;
        player.rb.velocity = v;

        player.UpdateFacing();
    }
}