using System;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idle;
    public Enemy_MoveState move;
    public Enemy_GroundedState grounded;

    public Enemy_AttackState attack;

    [Header("Player Detection")]
    [SerializeField]
    private Transform playerDetection;
    [SerializeField]
    private float playerDetectionLength = 5f;
    [SerializeField]
    private LayerMask playerLayer;

    private readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private readonly int TriggerAttackHash = Animator.StringToHash("TriggerAttack");

    protected override void Awake()
    {
        base.Awake();

        idle = new Enemy_IdleState(StateMachine, this);
        move = new Enemy_MoveState(StateMachine, this);
        attack = new Enemy_AttackState(StateMachine, this);
        grounded = new Enemy_GroundedState(StateMachine, this);
    }

    protected override void Update()
    {
        base.Update();

        anim.SetFloat(XVelocityHash, rb.velocity.x);
    }

    public virtual void OnAttackEnd()
    {
        StateMachine.ChangeState(move);
    }

    public void triggerAttack()
    {
        anim.SetTrigger(TriggerAttackHash);
    }

    protected override void Detect()
    {
        base.Detect();

        var raycastHit2D = Physics2D.Raycast(playerDetection.position,
            transform.right * GetFaceRightInt,
            playerDetectionLength,
            playerLayer);
        
        if (raycastHit2D.collider != null)
        {
            Debug.Log("Player detected");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(playerDetection.position,
            playerDetection.position +transform.right * GetFaceRightInt * playerDetectionLength);
    }
}