using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Slide")]
    [SerializeField]
    private float wallSlideSpeed = 3f;
    public float WallSlideSpeed => wallSlideSpeed;
    [SerializeField]
    private float fastSlideSpeed = 6f;
    public float FastSlideSpeed => fastSlideSpeed;

    [field: Header("Dash")]
    [field: SerializeField]
    public float DashSpeed { get; private set; } = 15f;

    [field: SerializeField] public float DashDuration { get; private set; } = .2f;

    [Header("Basic Attack Combo")]
    [SerializeField]
    private float basicAttackComboDuration = 0.3f;
    public float BasicAttackComboDuration => basicAttackComboDuration;
    [SerializeField]
    private Vector2[] basicAttackVelocity;
    public Vector2[] BasicAttackVelocity => basicAttackVelocity;


    // Animator Params
    private static readonly int XVelocityHash = Animator.StringToHash("x_velocity");
    private static readonly int YVelocityHash = Animator.StringToHash("y_velocity");
    private static readonly int IsJumpOrFallHash = Animator.StringToHash("IsJumpOrFall");
    private static readonly int IsSlideHash = Animator.StringToHash("IsSlide");
    private static readonly int IsIdleMoveHash = Animator.StringToHash("IsIdleMove");
    private static readonly int IsDashHash = Animator.StringToHash("IsDash");
    private static readonly int TriggerBasicAttackHash = Animator.StringToHash("TriggerBasicAttack");
    private static readonly int BasicAttackIndexHash = Animator.StringToHash("BasicAttackIndex");

    // State
    public Player_IdleState Idle { get; private set; }
    public Player_MoveState Move { get; private set; }
    public Player_JumpState Jump { get; private set; }
    public Player_FallState Fall { get; private set; }
    public Player_SlideState Slide { get; private set; }
    public Player_DashState Dash { get; private set; }
    public Player_BasicAttack BasicAttack { get; private set; }

    // 输入缓存
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool BasicAttackPressed { get; private set; }

    /// <summary>
    /// 墙壁检测
    /// </summary>
    public bool IsWallTouched { get; private set; }


    /// <summary>
    /// Input System Action
    /// </summary>
    private PlayerInputSet PlayerInputSet { get; set; }

    /// <summary>
    /// 基本攻击是否结束
    /// </summary>
    public bool IsBasicAttackEnd { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Idle = new Player_IdleState(StateMachine, this);
        Move = new Player_MoveState(StateMachine, this);
        Jump = new Player_JumpState(StateMachine, this);
        Fall = new Player_FallState(StateMachine, this);
        Slide = new Player_SlideState(StateMachine, this);
        Dash = new Player_DashState(StateMachine, this);
        BasicAttack = new Player_BasicAttack(StateMachine, this);

        PlayerInputSet = new PlayerInputSet();
    }

    protected override void Update()
    {
        base.Update();
        // 1. 输入采集
        JumpPressed = PlayerInputSet.Player.Jump.WasPressedThisFrame();
        DashPressed = PlayerInputSet.Player.Dash.WasPressedThisFrame();
        BasicAttackPressed = PlayerInputSet.Player.BasicAttack.WasPressedThisFrame();
        if (BasicAttackPressed)
        {
            StateMachine.ChangeState(BasicAttack);
        }

        // 2. 更新动画参数
        anim.SetFloat(XVelocityHash, rb.velocity.x);
        anim.SetFloat(YVelocityHash, rb.velocity.y);

        anim.SetBool(IsJumpOrFallHash, !IsGrounded && !IsWallTouched);
        anim.SetBool(IsSlideHash, IsWallTouched && rb.velocity.y < 0);

        // 3. 逻辑更新
        StateMachine.LogicUpdate();
    }

    protected override void Detect()
    {
        base.Detect();

        // 墙壁射线检测 -> WallSlide
        int faceRight = transform.localScale.x > 0 ? 1 : -1;
        var hit = Physics2D.Raycast(wallCheck.position,
            transform.right * faceRight,
            wallCheckLength,
            wallLayer);

        if (hit.collider && rb.velocity.y < 0)
            IsWallTouched = true;
        else
            IsWallTouched = false;
    }


    private void OnEnable()
    {
        PlayerInputSet.Enable();
        PlayerInputSet.Player.Movement.performed += OnMovement;
        PlayerInputSet.Player.Movement.canceled += OnMovement;
    }

    private void OnDisable()
    {
        PlayerInputSet.Player.Movement.performed -= OnMovement;
        PlayerInputSet.Player.Movement.canceled -= OnMovement;
        PlayerInputSet.Disable();
    }

    protected override void Start()
    {
        base.Start();
        // 状态机初始化
        StateMachine.Initialize(Idle);
    }

    public void OnBasicAttackStart()
    {
        IsBasicAttackEnd = false;
    }

    
    public override void OnBasicAttackEnd()
    {
        IsBasicAttackEnd = true;
    }

    // Animator 控制（供状态调用，保持集中）
    // public void SetAnimatorJumpFall(bool value) => _animator.SetBool(IsJumpOrFall, value);
    //
    public void SetIsDash(bool value) => anim.SetBool(IsDashHash, value);
    public void SetIdleMove(bool value) => anim.SetBool(IsIdleMoveHash, value);
    public void SetJumpFall(bool value) => anim.SetBool(IsJumpOrFallHash, value);
    public void SetBasicAttack() => anim.SetTrigger(TriggerBasicAttackHash);
    public void SetBasicAttackIndex(int index) => anim.SetInteger(BasicAttackIndexHash, index);

    public void UpdateFacing()
    {
        var xInput = MoveInput.x;
        if (Mathf.Abs(xInput) <= 0.01f) return;

        var s = transform.localScale;
        s.x = Math.Abs(s.x) * (xInput < 0 ? -1 : 1);
        transform.localScale = s;
    }


    private void OnMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
}