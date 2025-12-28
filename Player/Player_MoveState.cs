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

        if (Mathf.Abs(player.MoveInput.x) <= 0.01f)
        {
            stateMachine.ChangeState(player.Idle);
            return;
        }

        player.UpdateFacing();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        var v = player.rb.velocity;
        v.x = player.MoveInput.x * player.MoveSpeed;
        player.rb.velocity = v;
    }
}