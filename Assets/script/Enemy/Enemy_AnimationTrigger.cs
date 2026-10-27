using UnityEngine;

public class Enemy_AnimationTrigger : MonoBehaviour
{

    private Enemy enemy =>GetComponentInParent<Enemy>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    //检测玩家攻击范围内的所有碰撞体。遍历这些碰撞体，找到其中属于敌人的对象。对这些敌人调用Damage()方法，造成伤害。
    private void AttackTrigger()
    {
        //检测指定位置和半径范围内的所有2D碰撞体（Collider2D）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        //循环遍历colliders数组中的每一个碰撞体
        foreach (var hit in colliders)
        {
            //获取当前碰撞体hit所附加的Player组件,如果不为空则是玩家
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats _target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(_target);
                //hit.GetComponent<Player>().Damage();
            }
        }
    }

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();

}
