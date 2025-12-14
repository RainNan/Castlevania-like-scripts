using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 应该设置为私有的
    // public Rigidbody2D rb;
    // public float x_velocity;
    // public float y_velocity;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float x_velocity;
    private float y_velocity;
    public string playerName = "rain";

    [Header("Movement details")]
    // 仅对 Inspector 窗口有效
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    private bool _facingRight = true;
    private bool canMove = true;
    private bool canJump = true;


    /// <summary>
    /// 检查是否在地面上的小线段的[距离] 
    /// </summary>
    [Header("Collision details")] [SerializeField]
    private float groundCheckDistance;

    /// <summary>
    /// 是否在地面上
    /// </summary>
    private bool _isGrounded = true;

    // /// <summary>
    // /// 是否攻击状态
    // /// </summary>
    // private bool _isAttacking = false;

    /// <summary>
    /// 地面的 Layer 掩码
    /// </summary>
    [SerializeField] private LayerMask groundMask;
    
    [SerializeField] private LayerMask whatIsEnemy;


    /// <summary>
    /// 对应于 Animator 的 Parameters
    /// </summary>
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private static readonly int XVelocity = Animator.StringToHash("xVelocity");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int Attack = Animator.StringToHash("attackTrigger");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleCollision();
        HandleInput();
        HandleAnimations();
    }

    public void EnableMoveAndJump(bool enable) => (canJump, canMove) = (enable, enable);

    private void HandleCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
    }

    /// <summary>
    /// 处理 input
    /// </summary>
    private void HandleInput()
    {
        if (canMove)
        {
            x_velocity = Input.GetAxisRaw("Horizontal") * moveSpeed;
            Move(x_velocity);
        }
        else
            Move(0);


        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            TryToJump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) // Input.GetMouseDown(0)
        {
            TryToAttack();
        }
    }

    /// <summary>
    /// 试图攻击（有可能失败，如在跳跃时候）
    /// </summary>
    private void TryToAttack()
    {
        if (_isGrounded)
            anim.SetTrigger(Attack);
    }

    /// <summary>
    /// 动画处理
    /// </summary>
    private void HandleAnimations()
    {
        anim.SetFloat(XVelocity, rb.velocity.x);
        anim.SetFloat(YVelocity, rb.velocity.y);
        anim.SetBool(IsGrounded, _isGrounded);
    }

    private void Start()
    {
        Debug.Log("player [ " + playerName + "] enter the game!");
    }

    private void Move(float xVelocity)
    {
        if ((xVelocity > 0 && !_facingRight) || (xVelocity < 0 && _facingRight))
            Filp();

        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void TryToJump()
    {
        if (_isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    /// <summary>
    /// 人物改变面朝向
    /// </summary>
    [ContextMenu("Filp")]
    private void Filp()
    {
        transform.Rotate(0, 180, 0);
        _facingRight = !_facingRight;
    }

    /// <summary>
    /// 画线从而帮助我们确定 groundCheckDistance 的值
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
    }
}