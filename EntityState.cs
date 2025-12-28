using System;
using UnityEngine;

public class EntityState
{
    protected readonly StateMachine stateMachine;
    protected readonly Entity entity;

    protected readonly Rigidbody2D rb;

        
    protected float stateTimer = 3f;
    
    public EntityState(StateMachine stateMachine,Entity entity)
    {
        this.stateMachine = stateMachine;
        this.entity = entity;
 
        rb = entity.rb;
    }
    
    public virtual void Enter()
    {
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