using System;
using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Mathf.Abs(_player.MoveInput.x) <= 0.01f)
        {
            _stateMachine.ChangeState(_player.Idle);
            return;
        }

        _player.UpdateFacing();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        var v = _player.rb.velocity;
        v.x = _player.MoveInput.x * _player.MoveSpeed;
        _player.rb.velocity = v;
    }
}