using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [Header("Move")] [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;

    [Header("Jump")] [SerializeField] private float jumpForce = 8f;
    public float JumpForce => jumpForce;

    [Header("Slide")] [SerializeField] private float wallSlideSpeed = 3f;
    public float WallSlideSpeed => wallSlideSpeed;
    [SerializeField] private float fastSlideSpeed = 6f;
    public float FastSlideSpeed => fastSlideSpeed;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")] [SerializeField]
    private Transform wallCheck;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckLength = 0.2f;

    private Animator _animator;
    private Rigidbody2D _rb;
    public Rigidbody2D rb => _rb;

    // Animator Params
    private static readonly int XVelocity = Animator.StringToHash("x_velocity");
    private static readonly int YVelocity = Animator.StringToHash("y_velocity");
    private static readonly int IsJumpOrFall = Animator.StringToHash("IsJumpOrFall");
    private static readonly int IsSlide = Animator.StringToHash("IsSlide");
    private static readonly int IsGrounded_Hash = Animator.StringToHash("IsGrounded");


    public StateMachine StateMachine { get; private set; }
    public string currentStateName;

    // State
    public Player_IdleState Idle { get; private set; }
    public Player_MoveState Move { get; private set; }
    public Player_JumpState Jump { get; private set; }
    public Player_FallState Fall { get; private set; }
    public Player_SlideState Slide { get; private set; }

    // 输入缓存
    private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;
    private bool _jumpPressed;
    public bool JumpPressed => _jumpPressed;

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

        // 2. 更新动画参数
        _animator.SetFloat(XVelocity, _rb.velocity.x);
        _animator.SetFloat(YVelocity, _rb.velocity.y);
        _animator.SetBool(IsJumpOrFall, !IsGrounded && !IsWallTouched);
        _animator.SetBool(IsSlide, IsWallTouched && _rb.velocity.y < 0);
        _animator.SetBool(IsGrounded_Hash, IsGrounded);

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
    // public void SetIsSlide(bool value) => _animator.SetBool(IsSlide, value);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        int faceRight = transform.localScale.x > 0 ? 1 : -1;
        Gizmos.DrawLine(wallCheck.position,
            wallCheck.position + transform.right * faceRight * wallCheckLength);
    }
#endif
}