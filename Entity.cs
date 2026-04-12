using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    public Rigidbody2D rb { get; private set; }

    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    public Entity_DeathState EntityDeathState;

    [Header("Move")]
    [SerializeField]
    private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 8f;
    public float JumpForce => jumpForce;

    [Header("Ground Check")]
    [SerializeField]
    protected Transform groundCheck;
    [SerializeField]
    protected float groundCheckRadius = 0.15f;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField]
    protected Transform wallCheck;
    [SerializeField]
    protected LayerMask wallLayer;
    [SerializeField]
    protected float wallCheckLength = 0.2f;

    [SerializeField]
    protected float impulse = 1f;
    /// <summary>
    /// 当 knock back 的时候，暂停速度更新
    /// </summary>
    protected bool IsPauseFixedUpdate = false;

    protected StateMachine StateMachine { get; private set; }
    public string currentStateName;

    /// <summary>
    /// 战斗相关，hp atk 等
    /// </summary>
    [Header("Battle System")]
    [SerializeField]
    private float maxHp = 100f;
    [SerializeField]
    public float hp = 100f;
    [SerializeField]
    public float atk = 25f;
    public float MaxHp => maxHp;
    public float Hp => hp;
    public float Atk => atk;
    public float HpNormalized => maxHp <= 0f ? 0f : Mathf.Clamp01(hp / maxHp);
    public bool IsDead => hp <= 0f;
    public event Action<Entity> StatsChanged;
    public event Action<Entity> HealthChanged;
    public event Action<Entity> Died;
    private bool hasEnteredDeathState;


    /// <summary>
    /// 地面检测
    /// </summary>
    public bool IsGrounded { get; private set; }

    /// <summary>
    /// 墙壁检测
    /// </summary>
    public bool IsWallTouched { get; private set; }


    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StateMachine = new StateMachine();

        EntityDeathState = new Entity_DeathState(StateMachine, this);
    }


    protected virtual void Start()
    {
        maxHp = Mathf.Max(1f, maxHp);
        hp = Mathf.Clamp(hp, 0f, maxHp);
        RaiseStatsChanged();
    }

    protected virtual void Update()
    {
        // 逻辑更新
        StateMachine.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        if (IsPauseFixedUpdate)
            return;


        // 1. 地面检测
        Detect();

        // 2. 物理更新
        StateMachine.PhysicUpdate();

        if (hp <= 0)
            TryEnterDeathState();
    }


    protected virtual void Detect()
    {
        // 1. 地面圆形检测
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        // 2. 墙壁射线检测 -> WallSlide
        var hit = Physics2D.Raycast(wallCheck.position,
            Vector3.right * GetFaceRightSign,
            wallCheckLength,
            wallLayer);

        if (hit.collider is not null)
            IsWallTouched = true;
        else
            IsWallTouched = false;
    }


    /// <summary>
    /// 强制翻转
    /// </summary>
    public void Flip()
    {
        var s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }


    public bool IsFaceRight => transform.localScale.x > 0;
    public float GetFaceRightSign => Mathf.Sign(transform.localScale.x);


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck)
        {
            int faceRight = transform.localScale.x > 0 ? 1 : -1;
            Gizmos.DrawLine(wallCheck.position,
                wallCheck.position + transform.right * faceRight * wallCheckLength);
        }
    }
#endif
    public virtual void OnBasicAttackEnd()
    {
    }

    public virtual void OnAttack()
    {
    }

    protected bool ApplyDamage(float dmg)
    {
        if (hasEnteredDeathState)
            return true;

        var oldHp = hp;
        hp = Mathf.Clamp(hp - Mathf.Max(0f, dmg), 0f, maxHp);

        if (!Mathf.Approximately(oldHp, hp))
            HealthChanged?.Invoke(this);

        if (hp <= 0f)
        {
            TryEnterDeathState();
            return true;
        }

        return false;
    }

    protected void RaiseStatsChanged()
    {
        StatsChanged?.Invoke(this);
        HealthChanged?.Invoke(this);
    }

    private void TryEnterDeathState()
    {
        if (hasEnteredDeathState)
            return;

        hasEnteredDeathState = true;
        Died?.Invoke(this);
        StateMachine.ChangeState(EntityDeathState);
    }

    public void OnDead() => anim.SetBool(IsDeadHash, true);
}
