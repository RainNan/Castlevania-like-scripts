using System;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState idle;
    public Enemy_MoveState move;
    public Enemy_GroundedState grounded;
    public Enemy_BattleState battle;
    public Enemy_AttackState attack;

    /// <summary>
    ///  玩家处于这个距离内，则转入[战斗状态]
    /// </summary>
    [Header("Player Detection")]
    [SerializeField]
    private Transform playerDetecter;
    [SerializeField]
    private float playerDetectionLength = 5f;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    public float battleMoveSpeed = 5f;

    /// <summary>
    /// 玩家处于这个距离内，则转入[攻击状态]
    /// </summary>
    [Header("Battle")]
    [SerializeField]
    public float attackDistance = 2f;

    public RaycastHit2D raycastHit2D;

    private readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private readonly int TriggerAttackHash = Animator.StringToHash("TriggerAttack");
    private readonly int IsBattleHash = Animator.StringToHash("IsBattle");

    protected override void Awake()
    {
        base.Awake();

        RegisterStates();
    }

    /// <summary>
    /// 注册所有状态
    /// </summary>
    private void RegisterStates()
    {
        idle = new Enemy_IdleState(StateMachine, this);
        move = new Enemy_MoveState(StateMachine, this);
        attack = new Enemy_AttackState(StateMachine, this);
        grounded = new Enemy_GroundedState(StateMachine, this);
        battle = new Enemy_BattleState(StateMachine, this);
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

    #region set Animator Parameters

    public void triggerAttack() => anim.SetTrigger(TriggerAttackHash);

    public void setIsBattle(bool isBattle) => anim.SetBool(IsBattleHash, isBattle);

    #endregion

    protected override void Detect()
    {
        base.Detect();

        raycastHit2D = Physics2D.Raycast(playerDetecter.position,
            transform.right * GetFaceRightInt,
            playerDetectionLength,
            playerLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(playerDetecter.position,
            playerDetecter.position + transform.right * GetFaceRightInt * playerDetectionLength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerDetecter.position,
            playerDetecter.position + transform.right * GetFaceRightInt * attackDistance);
    }
}