using System.Collections;
using UnityEngine;
//获得相同的组件
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;//	LayerMask: 这是字段的类型，LayerMask 是一个用于指定物理层的结构体  ;   [SerializeField] 属性使得 whatIsPlayer 字段在 Unity 编辑器中可见
    [Header("眩晕参数")]//Stunned info
    public float stunDuration;
    public Vector2 stunDirection;
    public bool canBeStunned;
    [SerializeField] protected GameObject counterImage;


    [Header("移动参数")]//Move info
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("攻击参数")]//Attack info
    public float agroDistance = 2;
    public float attackDistance;
    public float attackCoolDown;
    public float minAttackCoolDown;//最小攻击冷却时间
    public float maxAttackCoolDown;
    [HideInInspector] public float lastTimeAttack;


    public EnemyStateMachine stateMachine { get; private set; }
    public EntityFX fx { get; private set; }
    private Player player;
    public string lastAnimBoolName { get; protected set; }// 存储上一个动画布尔值的名称


    protected override void Start()
    {
        base.Start();

        fx = GetComponent<EntityFX>();//使用EntityFX来控制玩家的特效
    }


    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();


    }


    public virtual void AssignLastBoolName(string _animBoolName) => lastAnimBoolName = _animBoolName;


    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }



    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }


    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)//IEnumerator 是一个接口，用于支持迭代器的实现。迭代器允许你在集合上进行迭代操作。Unity 中的协程（Coroutine）也使用 IEnumerator 来实现异步操作。
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);//等待 _seconds 秒

        FreezeTime(false);
    }


    #region Counter Attack Window

    public virtual void OpenCounterAttackWindow()// virtual 方法可以在派生类中使用 override 关键字进行重写，以提供不同的实现。
    {
        canBeStunned = true;
        counterImage.SetActive(true);//这行代码将 counterImage 游戏对象设置为激活状态，使其在游戏中可见。
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion


    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }


    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}