using System;
using UnityEngine;

public class Player_DashState : EntityState
{
    public Player_DashState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _stateTimer = _player.DashDuration;

        _player.SetIsDash(true);
        _player.SetIdleMove(false);
        _player.SetJumpFall(false);
    }

    public override void Exit()
    {
        base.Exit();
        _player.SetIsDash(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 0f)
        {
            _stateTimer = 0f;
            if (Math.Abs(_player.MoveInput.x) > 0.01f)
                _stateMachine.ChangeState(_player.Move);
            else
                _stateMachine.ChangeState(_player.Idle);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        var v = _player.rb.velocity;
        v.x = _player.MoveInput.x * _player.DashSpeed;
        _player.rb.velocity = v;
    }
}