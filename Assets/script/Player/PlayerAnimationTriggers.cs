using UnityEngine;


//这个脚本是装在player的孩子animator上的，使animator能够用player的AnimationTrigger函数

public class PlayerAnimationTriggers : MonoBehaviour
{

    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    //检测玩家攻击范围内的所有碰撞体。遍历这些碰撞体，找到其中属于敌人的对象。对这些敌人调用Damage()方法，造成伤害。
    private void AttackTrigger()
    {
        switch(player.primaryAttackState.comboCounter)
        {
            case 0: AudioManager.instance.PlaySFX(4 , null);break; //播放音效
            case 1: AudioManager.instance.PlaySFX(5, null); break; //播放音效
            case 2: AudioManager.instance.PlaySFX(6, null); break; //播放音效
            default: break;
        }

        //检测指定位置和半径范围内的所有2D碰撞体（Collider2D）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //循环遍历colliders数组中的每一个碰撞体
        foreach (var hit  in colliders)
        {
            //获取当前碰撞体hit所附加的Enemy组件,如果不为空则是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if(_target != null)
                {
                    player.stats.DoDamage(_target);
                }

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                if (weaponData != null)
                {
                    weaponData.Effect(_target.transform);
                }
                //hit.GetComponent<Enemy>().Damage();
                //hit.GetComponent<CharacterStats>().TakeDamage(player.stats.damage.GetValue());
                //Debug.Log(player.stats.damage.GetValue());
            }
        }
    }

    //创建要丢出来的剑
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreatSword();
    }
}
