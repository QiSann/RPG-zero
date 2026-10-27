using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Data", menuName = "Data/Item effect/Heal effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;
    public override void ExcuteEffect(Transform _enemyPositon)
    {
        //获得玩家状态栏
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //治疗的血量是多少
        int healAmont = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        //完成治疗效果
        playerStats.IncreaseHealthBy(healAmont);


    }
}