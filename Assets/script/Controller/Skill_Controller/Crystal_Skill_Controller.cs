using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    //不能使用private Animator anim = GetComponent<Animator>();是因为在字段初始化阶段调用 GetComponent，而此时 MonoBehaviour 的生命周期尚未开始
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRadomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius , whatIsEnemy);
        if(colliders.Length > 0 )
        {
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;

        }
    }


    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;
        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canMove && closestTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }


    private void AnimationExplodeEvent()
    {
        //检测指定位置和半径范围内的所有2D碰撞体（Collider2D）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        //循环遍历colliders数组中的每一个碰撞体
        foreach (var hit in colliders)
        {
            //获取当前碰撞体hit所附加的Enemy组件,如果不为空则是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerManager.instance.player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());
                //hit.GetComponent<Enemy>().DamageEffect();
                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);//获取护身符

                if (equipedAmulet != null)
                {
                    equipedAmulet.Effect(hit.transform);//如果有护身符，就对敌人造成额外伤害
                }
            }
        }
    }
    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            //它是一个布尔值（bool），但与普通的布尔参数不同，Trigger 参数在被设置为 true 后会自动重置为 false，并且只会触发一次
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    // 自毁
    public void SelfDestroy()
    {
        // 清除对目标的引用，防止后续访问已销毁的对象
        closestTarget = null;
        Destroy(gameObject);
    }
}
