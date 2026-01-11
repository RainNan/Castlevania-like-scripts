using UnityEngine;

public class Enemy : Entity, IDamageable
{
    public Enemy_IdleState Idle;
    public Enemy_MoveState Move;
    public Enemy_GroundedState Grounded;
    public Enemy_BattleState Battle;
    public Enemy_AttackState Attack;

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

        anim.SetFloat(XVelocityHash, rb.velocity.x);
    }

    public virtual void OnAttackEnd()
    {
        StateMachine.ChangeState(Move);
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
        
    }
#endif
    
    public void TakeDamage(float dmg)
    {
        TriggerTakeDamage();
        StateMachine.ChangeState(Move);

        IsPauseFixedUpdate = true;
        rb.AddForce(
            Vector2.right * -GetFaceRightSign * impulse,
            ForceMode2D.Impulse
        );
        
        hp -= dmg;
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

        if (hit.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(atk);
        }
    }

    public void OnTakeDamageEnd()
    {
        IsPauseFixedUpdate = false;
    }
}