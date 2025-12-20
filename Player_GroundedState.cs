using UnityEngine;

public class Player_GroundedState : EntityState
{
    public Player_GroundedState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // 地面按下跳跃：切到 JumpState
        if (_player.JumpPressed && _player.IsGrounded)
        {
            _stateMachine.ChangeState(_player.Jump);
            return;
        }
        
        // 如果不在地面（例如从平台边缘滑落），自动进 Fall
        if (!_player.IsGrounded)
        {
            _stateMachine.ChangeState(_player.Fall);
            return;
        }
    }
}