using UnityEngine;

public class Enemy : Entity, IDamageable
{
    public Enemy_IdleState Idle;
    public Enemy_MoveState Move;
    public Enemy_GroundedState Grounded;
    public Enemy_BattleState Battle;
    public Enemy_AttackState Attack;

    [Header("Patrol")]
    [SerializeField]
    private float patrolRange = 4f;
    private float patrolCenterX;
    public float PatrolLeftX => patrolCenterX - patrolRange;
    public float PatrolRightX => patrolCenterX + patrolRange;

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
    private Transform attackDetector;
    [SerializeField]
    public float attackRange = 2f;
    [SerializeField]
    public LayerMask playerLayerMask;

    public RaycastHit2D raycastHit2D;

    private readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private readonly int TriggerAttackHash = Animator.StringToHash("TriggerAttack");
    private readonly int IsBattleHash = Animator.StringToHash("IsBattle");
    private readonly int TriggerTakeDamageHash = Animator.StringToHash("TriggerTakeDamage");

    protected override void Awake()
    {
        base.Awake();

        RegisterStates();
    }

    protected override void Start()
    {
        base.Start();
        patrolCenterX = transform.position.x;
    }

    /// <summary>
    /// 注册所有状态
    /// </summary>
    private void RegisterStates()
    {
        Idle = new Enemy_IdleState(StateMachine, this);
        Move = new Enemy_MoveState(StateMachine, this);
        Attack = new Enemy_AttackState(StateMachine, this);
        Grounded = new Enemy_GroundedState(StateMachine, this);
        Battle = new Enemy_BattleState(StateMachine, this);
    }

    protected override void Update()
    {
        base.Update();
        
        if (rb.velocity.x < -0.01f)
            Debug.Log($"slideBack vx={rb.velocity.x}, pause={IsPauseFixedUpdate}, state={StateMachine.CurrentState?.GetType().Name}");


        anim.SetFloat(XVelocityHash, rb.velocity.x);
    }

    public virtual void OnAttackEnd()
    {
        if (IsDead)
            return;

        StateMachine.ChangeState(Move);
    }

    public bool ShouldFlipAtPatrolBoundary()
    {
        if (IsFaceRight)
            return transform.position.x >= PatrolRightX;

        return transform.position.x <= PatrolLeftX;
    }

    #region set Animator Parameters

    public void triggerAttack() => anim.SetTrigger(TriggerAttackHash);

    public void setIsBattle(bool isBattle) => anim.SetBool(IsBattleHash, isBattle);

    public void TriggerTakeDamage() => anim.SetTrigger(TriggerTakeDamageHash);

    #endregion

    protected override void Detect()
    {
        base.Detect();

        raycastHit2D = Physics2D.Raycast(playerDetecter.position,
            transform.right * GetFaceRightSign,
            playerDetectionLength,
            playerLayer);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(playerDetecter.position,
            playerDetecter.position + transform.right * GetFaceRightSign * playerDetectionLength);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerDetecter.position,
            playerDetecter.position + transform.right * GetFaceRightSign * attackRange);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(
            attackDetector.position,
            attackRange
        );

        var centerX = Application.isPlaying ? patrolCenterX : transform.position.x;
        var left = new Vector3(centerX - patrolRange, transform.position.y, transform.position.z);
        var right = new Vector3(centerX + patrolRange, transform.position.y, transform.position.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawWireSphere(left, 0.12f);
        Gizmos.DrawWireSphere(right, 0.12f);
        
    }
#endif
    
    public void TakeDamage(float dmg, float attackDir)
    {
        if (IsDead)
            return;

        TriggerTakeDamage();

        IsPauseFixedUpdate = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(
            Vector2.right * attackDir * impulse,
            ForceMode2D.Impulse
        );

        ApplyDamage(dmg);
        Debug.Log($"[{GetType().Name}] current hp [{hp}]");
    }
    

    public override void OnAttack()
    {
        base.OnAttack();
        
        var hit = Physics2D.OverlapCircle(
            attackDetector.position,
            attackRange,
            playerLayerMask
        );

        if (hit != null && hit.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(atk, GetFaceRightSign);
        }
    }

    public void OnTakeDamageEnd()
    {
        if (IsDead)
            return;

        rb.velocity = new Vector2(0f, rb.velocity.y);
        IsPauseFixedUpdate = false;
        
        StateMachine.ChangeState(Idle);
    }
}
