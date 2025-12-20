using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(StateMachine stateMachine, Player player) : base(stateMachine, player)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 更高的鲁棒性，适应于微小的输入，比如摇杆
        if (Mathf.Abs(_player.MoveInput.x) > 0.01f)
        {
            _stateMachine.ChangeState(_player.Move);
            return;
        }
    }

    public override void PhysicUpdate()
    {
        // Idle：只把水平速度收敛为 0，Y 交给物理
        var v = _player.rb.velocity;
        v.x = 0f;
        _player.rb.velocity = v;
    }
}