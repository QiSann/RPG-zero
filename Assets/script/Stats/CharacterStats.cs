using System.Collections;
using UnityEngine;

public enum StatType//枚举 StatType 的定义
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("主要数据统计")]
    public Stat strength;//力量,加伤害和暴击伤害
    public Stat agility;//敏捷，加闪避率和暴击率
    public Stat intelligence;//智力，加魔法伤害和魔法抗性
    public Stat vitality;//体力，加血量上限

    [Header("攻击数据统计")]
    public Stat damage;//伤害
    public Stat critChance;//暴击率
    public Stat critPower;//暴击伤害

    [Header("防御数据统计")]
    public Stat maxHealth;//血量上限
    public Stat armor;//护甲
    public Stat evasion;//闪避
    public Stat magicResistance;//魔抗

    [Header("魔法数据")]
    public Stat fireDamage;//火
    public Stat iceDamage;//冰
    public Stat lightningDamage;//闪电

    //用于判断是否着火了、被冻住了、被眩晕了
    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;


    [SerializeField] private float ailmentDuration;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead {  get; private set; }
    private bool isInvincible;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        if (shockedTimer < 0)
        {
            isShocked = false;
        }
        if(isIgnited)
        {
            ApplyIgniteDamage();
        }
    }
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier , _duration , _statToModify));
    }
    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _StatToModify)
    {
        _StatToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _StatToModify.RemoveModifier(_modifier);
    }
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if(_targetStats.isInvincible)
        {
            return;
        }

        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        //DoMagicDamage(_targetStats); //如果想要普通攻击有元素，就用这个
    }
    public virtual void TakeDamage(int _damage)
    {
        if(isInvincible)
        {
            return;
        }

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFx");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    #region Magic damage and ailments
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;
    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightningDamage.GetValue();

        Debug.Log("火" + _fireDamage + "冰" + _iceDamage + "电" + _lightingDamage);
        int totleMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totleMagicalDamage = CheckTargetResistance(_targetStats, totleMagicalDamage);

        _targetStats.TakeDamage(totleMagicalDamage);

        //防止循环在所有元素伤害为0时出现死循环
        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        //让元素效果取决与伤害


        //为了防止出现元素伤害一致而导致无法触发元素效果
        //循环判断触发某个元素效果

        AttemptyToApplyAilement(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }
    private void AttemptyToApplyAilement(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _lightingDamage && _iceDamage > _fireDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;



        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .25f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .40f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .55f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

        }


        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        //给点燃伤害赋值

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }//造成元素效果
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentDuration;

            fx.IgniteFxFor(ailmentDuration);
        }
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = ailmentDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentDuration);
            fx.ChillFxFor(ailmentDuration);
        }
        if (_shock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)//防止出现敌人使玩家进入shock状态后也出现闪电
                    return;
                HitNearestTargetWithShockStrike();
            }//isShock为真时反复执行的函数为寻找最近的敌人，创建闪电实例并传入数据
        }
    }
    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentDuration;

        fx.ShockFxFor(ailmentDuration);
    }
    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);//找到环绕自己的所有碰撞器

        float closestDistance = Mathf.Infinity;//正无穷大的表示形式（只读）
        Transform closestEnemy = null;


        //https://docs.unity3d.com/cn/current/ScriptReference/Mathf.Infinity.html
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)// 防止最近的敌人就是Shock状态敌人自己
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);//拿到与敌人之间的距离
                if (distanceToEnemy < closestDistance)//比较距离，如果离得更近，保存这个敌人的位置,更改最近距离
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }
    #endregion
    #region Stat calculations
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        //这里的意思是如果我是被震慑住了，则对方增加20的闪避率
        if (isShocked)
        {
            totalEvasion += 20;
        }
        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }
        return false;
    }
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamge = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamge);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }//获取最大生命值
    #endregion
    public virtual void IncreaseHealthBy(int _amount)//加血函数
    {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }
    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;

        if(_damage > 0)
        {
            fx.CreatePopUpText(_damage.ToString());
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }
    protected virtual void Die()
    {
        isDead = true;
    }
    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvencible(bool _invincible) => isInvincible = _invincible;
    public Stat GetStat(StatType _statType)//根据 buffType 返回需要修改的属性
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightningDamage;

        else return null;
    }
}