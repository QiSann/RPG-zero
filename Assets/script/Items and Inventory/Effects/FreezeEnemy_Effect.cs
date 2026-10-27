using UnityEngine;


[CreateAssetMenu(fileName = "Freeze enemy Effect", menuName = "Data/Item effect/Freeze enemy")]
public class FreezeEnemy_Effect : ItemEffect
{

    [SerializeField] private float duration;

    public override void ExcuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();//玩家状态检查

        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .1f)
            return;

        if (!Inventory.instance.CanUseArmor())//检查玩家防具可用性
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);//寻找并冻结敌人

        foreach (var hit in colliders)//敌人冻结逻辑
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);//?是为了保证有这个组件

        }
    }
}