using System;
using UnityEngine;

public class Player_DashState : PlayerState
{
    public Player_DashState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.DashDuration;

        player.SetIsDash(true);
        player.SetIdleMove(false);
        player.SetJumpFall(false);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetIsDash(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            stateTimer = 0f;
            if (Math.Abs(player.MoveInput.x) > 0.01f)
                stateMachine.ChangeState(player.Move);
            else
                stateMachine.ChangeState(player.Idle);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        var v = player.rb.velocity;
        v.x = player.MoveInput.x * player.DashSpeed;
        player.rb.velocity = v;
    }
}