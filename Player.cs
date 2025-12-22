using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField]
    private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;

    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 8f;
    public float JumpForce => jumpForce;

    [Header("Slide")]
    [SerializeField]
    private float wallSlideSpeed = 3f;
    public float WallSlideSpeed => wallSlideSpeed;
    [SerializeField]
    private float fastSlideSpeed = 6f;
    public float FastSlideSpeed => fastSlideSpeed;

    [Header("Dash")]
    [SerializeField]
    private float _dashSpeed = 15f;
    public float DashSpeed => _dashSpeed;
    [SerializeField]
    private float _dashDuration = .2f;
    public float DashDuration => _dashDuration;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.15f;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private float wallCheckLength = 0.2f;
    
    [Header("Basic Attack Combo")]
    [SerializeField]
    private float basicAttackComboDuration = 0.3f;
    public float BasicAttackComboDuration => basicAttackComboDuration;
    [SerializeField]
    private Vector2[] basicAttackVelocity;
    public Vector2[] BasicAttackVelocity => basicAttackVelocity;

    private Animator _animator;
    private Rigidbody2D _rb;
    public Rigidbody2D rb => _rb;

    // Animator Params
    private static readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private static readonly int YVelocityHash = Animator.StringToHash("y_velocity");
    private static readonly int IsJumpOrFallHash = Animator.StringToHash("IsJumpOrFall");
    private static readonly int IsSlideHash = Animator.StringToHash("IsSlide");
    private static readonly int IsIdleMoveHash = Animator.StringToHash("IsIdleMove");
    private static readonly int IsDashHash = Animator.StringToHash("IsDash");
    private static readonly int TriggerBasicAttackHash = Animator.StringToHash("TriggerBasicAttack");
    private static readonly int BasicAttackIndexHash = Animator.StringToHash("BasicAttackIndex");


    public StateMachine StateMachine { get; private set; }
    public string currentStateName;

    // State
    public Player_IdleState Idle { get; private set; }
    public Player_MoveState Move { get; private set; }
    public Player_JumpState Jump { get; private set; }
    public Player_FallState Fall { get; private set; }
    public Player_SlideState Slide { get; private set; }
    public Player_DashState Dash { get; private set; }
    public Player_BasicAttack BasicAttack { get; private set; }

    // 输入缓存
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;
    private bool _jumpPressed;
    public bool JumpPressed => _jumpPressed;
    private bool _dashPressed;
    public bool DashPressed => _dashPressed;
    private bool _basicAttackPressed;
    public bool BasicAttackPressed => _basicAttackPressed;

    /// <summary>
    /// 地面检测结果
    /// </summary>
    public bool IsGrounded { get; private set; }

    /// <summary>
    /// 墙壁检测
    /// </summary>
    public bool IsWallTouched { get; private set; }

    /// <summary>
    /// Input System Action
    /// </summary>
    private PlayerInputSet _playerInputSet;

    public PlayerInputSet PlayerInputSet => _playerInputSet;

    /// <summary>
    /// 基本攻击是否结束
    /// </summary>
    private bool _isBasicAttackEnd = true;

    public bool IsBasicAttackEnd => _isBasicAttackEnd;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();

        StateMachine = new StateMachine();
        Idle = new Player_IdleState(StateMachine, this);
        Move = new Player_MoveState(StateMachine, this);
        Jump = new Player_JumpState(StateMachine, this);
        Fall = new Player_FallState(StateMachine, this);
        Slide = new Player_SlideState(StateMachine, this);
        Dash = new Player_DashState(StateMachine, this);
        BasicAttack = new Player_BasicAttack(StateMachine, this);

        _playerInputSet = new PlayerInputSet();
    }

    private void OnEnable()
    {
        _playerInputSet.Enable();
        _playerInputSet.Player.Movement.performed += OnMovement;
        _playerInputSet.Player.Movement.canceled += OnMovement;
    }

    private void OnDisable()
    {
        _playerInputSet.Player.Movement.performed -= OnMovement;
        _playerInputSet.Player.Movement.canceled -= OnMovement;
        _playerInputSet.Disable();
    }


    private void Start()
    {
        // 状态机初始化
        StateMachine.Initialize(Idle);
    }

    private void Update()
    {
        // 1. 输入采集
        _jumpPressed = _playerInputSet.Player.Jump.WasPressedThisFrame();
        _dashPressed = _playerInputSet.Player.Dash.WasPressedThisFrame();
        _basicAttackPressed = _playerInputSet.Player.BasicAttack.WasPressedThisFrame();


        if (_basicAttackPressed)
        {
            StateMachine.ChangeState(BasicAttack);
        }

        // 2. 更新动画参数
        _animator.SetFloat(XVelocityHash, _rb.velocity.x);
        _animator.SetFloat(YVelocityHash, _rb.velocity.y);

        _animator.SetBool(IsJumpOrFallHash, !IsGrounded && !IsWallTouched);
        _animator.SetBool(IsSlideHash, IsWallTouched && _rb.velocity.y < 0);

        // 3. 逻辑更新
        StateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        // 1. 地面检测
        Detect();

        // 2. 物理更新
        StateMachine.PhysicUpdate();
    }


    private void OnMovement(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void Detect()
    {
        // 1. 地面圆形检测
        if (!groundCheck || !wallCheck)
        {
            Debug.LogError("has no groundCheck or wallCheck!");
            return;
        }

        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        // 2. 墙壁射线检测
        int faceRight = transform.localScale.x > 0 ? 1 : -1;
        var hit = Physics2D.Raycast(wallCheck.position,
            transform.right * faceRight,
            wallCheckLength,
            wallLayer);

        if (hit.collider && _rb.velocity.y < 0)
            IsWallTouched = true;
        else
            IsWallTouched = false;
    }

    public void UpdateFacing()
    {
        var xInput = _moveInput.x;
        if (Mathf.Abs(xInput) <= 0.01f) return;

        var s = transform.localScale;
        s.x = Math.Abs(s.x) * (xInput < 0 ? -1 : 1);
        transform.localScale = s;
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

    // Animator 控制（供状态调用，保持集中）
    // public void SetAnimatorJumpFall(bool value) => _animator.SetBool(IsJumpOrFall, value);
    //
    public void SetIsDash(bool value) => _animator.SetBool(IsDashHash, value);
    public void SetIdleMove(bool value) => _animator.SetBool(IsIdleMoveHash, value);
    public void SetJumpFall(bool value) => _animator.SetBool(IsJumpOrFallHash, value);
    public void SetBasicAttack() => _animator.SetTrigger(TriggerBasicAttackHash);
    public void SetBasicAttackIndex(int index) => _animator.SetInteger(BasicAttackIndexHash, index);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        int faceRight = transform.localScale.x > 0 ? 1 : -1;
        Gizmos.DrawLine(wallCheck.position,
            wallCheck.position + transform.right * faceRight * wallCheckLength);
    }
#endif

    public void OnBasicAttackStart()
    {
        _isBasicAttackEnd = false;
    }

    public void OnBasicAttackEnd()
    {
        _isBasicAttackEnd = true;
    }
    
    public bool IsFaceRight => transform.localScale.x > 0;
    public int GetFaceRightInt => IsFaceRight ? 1 : -1;
}