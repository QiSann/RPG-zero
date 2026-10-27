using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,//武器
    Armor,//防具
    Amulet,//项链
    Flask//药水
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]//在资源管理器中创建新的Item，创建的物品自带这个脚本
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("独特效果")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;
    [TextArea]
    public string itemEffectDescription;//独特效果描述


    [Header("主要数据统计")]
    public int strength;//力量,加伤害和暴击伤害
    public int agility;//敏捷，加闪避率和暴击率
    public int intelligence;//智力，加魔法伤害和魔法抗性
    public int vitality;//体力，加血量上限

    [Header("攻击数据统计")]
    public int damage;//伤害
    public int critChance;//暴击率
    public int critPower;//暴击伤害

    [Header("防御数据统计")]
    public int maxHealth;//血量上限
    public int armor;//护甲
    public int evasion;//闪避
    public int magicResistance;//魔抗

    [Header("魔法数据")]
    public int fireDamage;//火
    public int iceDamage;//冰
    public int lightningDamage;//闪电

    [Header("制作需求")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    public void Effect(Transform _enemyPositon)//执行物品效果
    {
        foreach (var item in itemEffects)
        {
            item.ExcuteEffect(_enemyPositon);
        }

    }

    public override string GetDescription()
    {
        sb.Length = 0;//确保每次调用时是从头开始
        descriptionLength = 0;//重置描述的属性数量

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力");
        AddItemDescription(vitality, "活力");

        AddItemDescription(damage, "伤害");
        AddItemDescription(critChance, "暴击率");
        AddItemDescription(critPower, "暴击伤害");

        AddItemDescription(maxHealth, "生命值");
        AddItemDescription(armor, "护甲");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(magicResistance, "魔抗");

        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "冰霜伤害");
        AddItemDescription(lightningDamage, "闪电伤害");

        if (descriptionLength < 5)//确保最终返回的描述文本至少有 5 行内容
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");//空行
            }
        }

        if (itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }

        return sb.ToString();//返回描述文本
    }

    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)//判断该属性没有实际值
        {
            if (sb.Length > 0)
                sb.AppendLine();//换行


            if (_value > 0)
                //sb.Append(_name+";"+_value);
                sb.Append("+ " + _value + " " + _name);//添加属性描述


            descriptionLength++;//每成功添加一个属性
        }
    }
}
