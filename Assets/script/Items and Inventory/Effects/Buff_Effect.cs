using UnityEngine;

[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item effect/Buff effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;// Buff 的类型
    [SerializeField] private int buffAmount; // Buff 的增加量
    [SerializeField] private float buffDuration;// Buff 持续时间


    public override void ExcuteEffect(Transform _enemyPositon)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();//获取当前玩家角色的 PlayerStats 组件，用来访问和修改角色的各项属性。
        stats.IncreaseStatBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }

}