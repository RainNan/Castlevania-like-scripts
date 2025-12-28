using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    public Rigidbody2D rb { get; private set; }


    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;
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


    protected StateMachine StateMachine { get; private set; }
    public string currentStateName;


    /// <summary>
    /// 地面检测结果
    /// </summary>
    public bool IsGrounded { get; private set; }


    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StateMachine = new StateMachine();
    }


    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        // 逻辑更新
        StateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        // 1. 地面检测
        Detect();

        // 2. 物理更新
        StateMachine.PhysicUpdate();
    }


    protected virtual void Detect()
    {
        // 1. 地面圆形检测
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
    public int GetFaceRightInt => IsFaceRight ? 1 : -1;


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
}