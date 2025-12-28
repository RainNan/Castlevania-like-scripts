using System;
using UnityEngine;

public class Player_SlideState : PlayerState
{
    
    public Player_SlideState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
            player.SetIdleMove(false);
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsGrounded)
        {
            if (Math.Abs(player.MoveInput.x) > 0.01f)
                stateMachine.ChangeState(player.Move);
            else
                stateMachine.ChangeState(player.Idle);
            return;
        }

        // 不再贴墙 -> 回空中下落
        if (!player.IsWallTouched)
        {
            stateMachine.ChangeState(player.Fall);
            return;
        }

        if (player.JumpPressed)
        {
            stateMachine.ChangeState(player.Jump);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        var v = player.rb.velocity;
        // 1. 贴墙也可以水平移动
        v.x = player.MoveInput.x * player.MoveSpeed;
        
        // 2. 下落速度是固定的, 按住s可以下落更快
        
        // limit = -10, v.y = -5
        float limit = (player.MoveInput.y < 0f) ? player.FastSlideSpeed : player.WallSlideSpeed;
        v.y = Mathf.Max(v.y, -limit);
        
        // 一起更新
        player.rb.velocity = v;
    }
}