using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;

    [Header("详细等级")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)] //用于在 Unity 的 Inspector 面板中为变量创建一个滑动条
    [SerializeField] private float percentageModfier = .4f;

    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModfiers();
        
        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    protected override void Update()
    {
        base.Update();
    }
    private void ApplyLevelModfiers()//根据等级修正数值
    {
        Modify(damage);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);


        Modify(damage);
        //Modify(critChance);
        //Modify(critPower);


        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);

        Modify(soulsDropAmount);

    }

    private void Modify(Stat _stat)//修正数值
    {
        for (int i = 0; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModfier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();

        Destroy(gameObject, 5f);
    }
}
