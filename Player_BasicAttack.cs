using UnityEngine;

public class Player_BasicAttack : EntityState
{
    public Player_BasicAttack(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _player.SetBasicAttack();
        _player.OnBasicAttackStart();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (_player.IsBasicAttackEnd)
        {
            _stateMachine.ChangeState(_player.Idle);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        
        var v = _player.rb.velocity;
        v.x = 0;
        _player.rb.velocity = v;
    }
}