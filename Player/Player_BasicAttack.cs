using UnityEngine;

public class Player_BasicAttack : PlayerState
{
    private const int FirstBasicAttackIndex = 1;
    private const int BasicAttackIndexMax = 3;

    private int _curBasicAttackIndex = FirstBasicAttackIndex;
    private float _lastBasicAttackTime;

    public Player_BasicAttack(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        ResetBasicAttackIndex();

        player.SetBasicAttackIndex(_curBasicAttackIndex);
        player.SetBasicAttack();
        player.OnBasicAttackStart();
    }

    public override void Exit()
    {
        base.Exit();
        _lastBasicAttackTime = Time.time;
        _curBasicAttackIndex++;
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsBasicAttackEnd)
        {
            stateMachine.ChangeState(player.Idle);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        var v = player.rb.velocity;
        v.x = player.BasicAttackVelocity[_curBasicAttackIndex - 1].x * player.GetFaceRightInt;
        player.rb.velocity = v;
    }

    /// <summary>
    /// 如果攻击索引超过最大索引值，则重置为第一个攻击索引
    /// 或combo持续时间结束后，重置攻击索引为第一个攻击索引
    /// </summary>
    private void ResetBasicAttackIndex()
    {
        if (Time.time > _lastBasicAttackTime + player.BasicAttackComboDuration)
        {
            _curBasicAttackIndex = FirstBasicAttackIndex;
            return;
        }

        if (_curBasicAttackIndex > BasicAttackIndexMax) _curBasicAttackIndex = FirstBasicAttackIndex;
    }
}