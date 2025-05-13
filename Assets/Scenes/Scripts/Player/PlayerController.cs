using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Attack")]
    [SerializeField] private AttackHitBox hitBoxController;
    [SerializeField] private float attackDuration = 0.5f;
    private bool canAttack = true;


    // Input System 相关变量
    private PlayerInputActions playerInputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;
    private bool isGrounded;

    

    private IEnumerator PerformAttack()
    {
        if (!canAttack) yield break;

        
        anim.SetTrigger("Attack"); // 触发攻击动画
        
    
        // 等待动画到达有效帧（通过动画事件激活碰撞体）
        yield return new WaitForSeconds(0.2f); 
        hitBoxController.Activate();
    
        // 攻击持续
        yield return new WaitForSeconds(attackDuration);
        hitBoxController.Deactivate();
    
        
    }

    private void Awake()
    {
        // 初始化组件
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 初始化 Input System
        playerInputActions = new PlayerInputActions();
        moveAction = playerInputActions.Player.Move;
        jumpAction = playerInputActions.Player.Jump;
        attackAction = playerInputActions.Player.Attack;
    }

    private void OnEnable()
    {
        // 启用输入并绑定跳跃事件
        playerInputActions.Player.Enable();
        jumpAction.performed += OnJump;
        
        attackAction.performed += OnAttack;
        playerInputActions.Player.Attack.Enable();
    }

    private void OnDisable()
    {
        // 禁用输入并解绑事件
        playerInputActions.Player.Disable();
        jumpAction.performed -= OnJump;
        attackAction.performed -= OnAttack;
    }

    private void Update()
    {
        // 获取输入
        moveInput = moveAction.ReadValue<Vector2>();

        // 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 移动逻辑
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // 攻击逻辑
        canAttack = isGrounded;

        // 动画控制
        anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("YVelocity", rb.linearVelocity.y);
        anim.SetBool("CanAttack", canAttack);

        // 角色转向（根据输入翻转Sprite）
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
        }

        
        
    }

    // 跳跃输入回调
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
            anim.SetBool("IsGrounded", false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            isGrounded = true;
            anim.SetBool("IsGrounded", true);
        }
    }

    // 可视化地面检测范围（仅Editor可见）
    private void OnDrawGizmosSelected()
    {
        // 添加判空保护，避免未赋值时报错
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        StartCoroutine(PerformAttack());
    }
    
}
