using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetIdleMove(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetIdleMove(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.DashPressed)
        {
            stateMachine.ChangeState(player.Dash);
            return;
        }

        // 地面按下跳跃：切到 JumpState
        if (player.JumpPressed && player.IsGrounded)
        {
            stateMachine.ChangeState(player.Jump);
            return;
        }

        // 如果不在地面（例如从平台边缘滑落），自动进 Fall
        if (!player.IsGrounded)
        {
            stateMachine.ChangeState(player.Fall);
            return;
        }
    }
}