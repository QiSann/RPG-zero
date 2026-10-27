using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLosingSpeed;

    private float cloneTimer;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    
    private Transform closestEnemy;
    private int facingDir = 1;

    private bool canDuplicateClone;
    private float chanceToDuplicate;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));

            if(sr.color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void SetupClone(Transform _newTransform , float _cloneDuration , bool _canAttack , Vector3 _offset , Transform _closestEnemy , bool _canDuplicateClone , float _chanceToDuplicate)
    {
        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        }
        transform.position = _newTransform.position  + _offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        FaceClosestTarget();

    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    //检测玩家攻击范围内的所有碰撞体。遍历这些碰撞体，找到其中属于敌人的对象。对这些敌人调用Damage()方法，造成伤害。
    private void AttackTrigger()
    {
        //检测指定位置和半径范围内的所有2D碰撞体（Collider2D）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        //循环遍历colliders数组中的每一个碰撞体
        foreach (var hit in colliders)
        {
            //获取当前碰撞体hit所附加的Enemy组件,如果不为空则是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                PlayerManager.instance.player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                //hit.GetComponent<Enemy>().DamageEffect();

                if(canDuplicateClone)//如果可以多重影分身
                {
                    if(Random.Range(0 , 100) < chanceToDuplicate )
                    {
                        SkillManager.instance.clone.CreatClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
 
        //改变分身的朝向
        if(closestEnemy != null)
        {
            if(transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
