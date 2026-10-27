using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{

    [Header("受击信息")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField] protected Vector2 knockbackOffset;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    [Header("碰撞信息")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("攻击信息")]
    public Transform attackCheck;
    public float attackCheckRadius;


    #region component
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd {  get; private set; }
    #endregion

    public int knockbackDir {  get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    //Action是一个System库自带的已经帮你声明好的委托,Action没有返回值、Func有返回值
    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        fx = GetComponent<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage , float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)//用于判断角色的受击方向以及改变角色朝向
    {
        if(_damageDirection.position.x > transform.position.x)
        {
            knockbackDir = -1;
        }
        else if (_damageDirection.position.x < transform.position.x)
        {
            knockbackDir = 1;
        }

    }
    public void SetupKnockbackPower(Vector2 _knockbackPower) => knockbackPower = _knockbackPower;//击退力

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        float xOffset = Random.Range(knockbackOffset.x , knockbackOffset.y);

        rb.linearVelocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked  = false;
        SetZeroVelocity();
    }

    #region velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
        {
            return;
        }
        rb.linearVelocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if(isKnocked)
        {
            return;
        }

        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion
    #region collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround); //检测地面
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);//检测墙壁

    //划线作用，能够在unity看到画的线
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position , attackCheckRadius); //绘制线框球体，值为中心和半径
    }
    #endregion
    #region flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if(onFlipped != null)
        {
            onFlipped();
        }
    }
    //翻转控制器
    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }

    public virtual void SetupDefaultFacingDir(int _direction)
    {
        facingDir = _direction;

        if (facingDir == -1)
            facingRight = false;
    }
    #endregion

    public virtual void Die()
    {

    }
}
