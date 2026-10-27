using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float returnSpeed = 12;
    private float FreezeTimeDuration;

    [Header("穿刺信息")]
    [SerializeField] private float pierceAmount;

    [Header("弹射信息")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("旋转攻击信息")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();

        enemyTarget = new List<Transform>();
    }


    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    //设置剑的初始方向、重力比例和玩家引用,如果穿刺次数为0，开启旋转动画
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _FreezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        FreezeTimeDuration = _FreezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.linearVelocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
        {
            anim.SetBool("Rotation", true);
        }

        Invoke("DestroyMe", 7);
        //Mathf.Clamp 是 Unity 提供的一个数学工具函数，用于将一个值限制在指定的范围内,它的语法是：Mathf.Clamp(value, min, max)
        spinDirection = Mathf.Clamp(rb.linearVelocity.x, -1, 1);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldwon)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldwon;
    }

    //让剑返回玩家，设置刚体约束并取消父对象
    public void ReturnSword()
    {
        //Rigidbody2D 组件的 constraints 属性用于设置刚体的约束条件
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
        {
            //transform.right世界空间中变换的红轴,即X轴，可以调整旋转
            transform.right = rb.linearVelocity;
        }

        if (isReturning)
        {
            isBouncing = false;
            anim.SetBool("Rotation", true);
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                anim.SetBool("Rotation", false);
                player.CatchTheSword();
            }
        }

        BounceLogic();
        SpinLogic();
    }

    //如果剑离玩家超过最大距离且未停止，调用 StopWhenSpinning() 停止剑,如果剑已停止，计时并周期性检测周围敌人进行攻击，直到旋转时间结束，剑开始返回
    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if (spinTimer <= 0)
        {
            spinTimer = spinDuration;
        }
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if (bounceAmount < 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    //Unity内置的物理触发器事件函数，当一个带有Collider2D的对象进入另一个带有Collider2D且启用了触发器的对象时，会调用这个函数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
        {
            return;
        }
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
        enemy.FreezeTimeFor(FreezeTimeDuration);
        //enemy.StartCoroutine("FreezeTimerFor", FreezeTimeDuration);

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);//获取护身符

        if (equipedAmulet != null)
            equipedAmulet.Effect(enemy.transform);//如果有护身符，就对敌人造成额外伤害
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.bodyType = RigidbodyType2D.Kinematic;// 将刚体类型设置为运动学,2D刚体需要移动，但不接受力的作用
        rb.constraints = RigidbodyConstraints2D.FreezeAll;// 冻结所有物理模拟

        if (isBouncing && enemyTarget.Count > 0)
        {
            return;
        }
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;// 将当前物体设置为碰撞物体的子物体
    }
}
