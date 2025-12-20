using System;
using UnityEngine;

public class Player_SlideState : EntityState
{
    
    public Player_SlideState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (_player.IsGrounded)
        {
            if (Math.Abs(_player.MoveInput.x) > 0.01f)
                _stateMachine.ChangeState(_player.Move);
            else
                _stateMachine.ChangeState(_player.Idle);
            return;
        }

        // 不再贴墙 -> 回空中下落
        if (!_player.IsWallTouched)
        {
            _stateMachine.ChangeState(_player.Fall);
            return;
        }

        if (_player.JumpPressed)
        {
            _stateMachine.ChangeState(_player.Jump);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        var v = _player.rb.velocity;
        // 1. 贴墙也可以水平移动
        v.x = _player.MoveInput.x * _player.MoveSpeed;
        
        // 2. 下落速度是固定的, 按住s可以下落更快
        
        // limit = -10, v.y = -5
        float limit = (_player.MoveInput.y < 0f) ? _player.FastSlideSpeed : _player.WallSlideSpeed;
        v.y = Mathf.Max(v.y, -limit);
        
        // 一起更新
        _player.rb.velocity = v;
    }
}