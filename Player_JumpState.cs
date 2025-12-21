
public class Player_JumpState : EntityState
{
    public Player_JumpState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        var v = _player.rb.velocity;
        v.y = _player.JumpForce;
        _player.rb.velocity = v;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
               
        // => Dash
        if (_player.DashPressed)
        {
            _stateMachine.ChangeState(_player.Dash);
            return;
        }

        if (_player.IsWallTouched)
        {
            _stateMachine.ChangeState(_player.Slide);
            return;
        }
        
        // 上升结束 -> Fall
        if (_player.rb.velocity.y <= 0f)
        {
            _stateMachine.ChangeState(_player.Fall);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        // 空中也允许水平控制
        var v = _player.rb.velocity;
        v.x = _player.MoveInput.x * _player.MoveSpeed;
        _player.rb.velocity = v;

        _player.UpdateFacing();
    }
}