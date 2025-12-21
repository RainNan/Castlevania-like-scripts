using UnityEngine;

public abstract class EntityState
{
    protected readonly StateMachine _stateMachine;
    protected readonly Player _player;
    protected float _stateTimer = 3f;

    protected EntityState(StateMachine stateMachine, Player player)
    {
        _stateMachine = stateMachine;
        _player = player;
    }

    public virtual void Enter()
    {
        _player.currentStateName = GetType().Name;
    }

    public virtual void Exit()
    {
    }

    /// <summary>
    /// 每帧逻辑（输入、切换状态、动画触发等）
    /// </summary>
    public virtual void LogicUpdate()
    {
    }

    /// <summary>
    /// 固定帧物理（rb.velocity / AddForce 等）
    /// </summary>
    public virtual void PhysicUpdate()
    {
    }
}