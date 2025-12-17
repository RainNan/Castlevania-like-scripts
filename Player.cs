using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }

    public Player_IdleState Idle { get; private set; }
    public Player_MoveState Move { get; private set; }

    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField] private float moveSpeed = 5f;

    public float MoveSpeed => moveSpeed;
    //public float MoveSpeed
    // {
    //     get { return moveSpeed; }
    // }
    
    [SerializeField] private Vector2 moveInput;

    /// <summary>
    /// 移动输入
    /// </summary>
    public Vector2 MoveInput => moveInput;
    
    /// <summary>
    /// Input System Action
    /// </summary>
    private PlayerInputSet _playerInputSet;

    private void Awake()
    {
        StateMachine = new StateMachine();

        Idle = new Player_IdleState(StateMachine, this);
        Move = new Player_MoveState(StateMachine, this);

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

    private void OnMovement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Start()
    {
        StateMachine.Initialize(Idle);
    }

    private void Update()
    {
        StateMachine.CurrentState?.Update(); // 空值保护
    }
}