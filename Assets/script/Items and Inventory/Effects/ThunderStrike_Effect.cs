using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]

public class TunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;//定义预制件


    public override void ExcuteEffect(Transform _enemyPositon)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPositon.position, Quaternion.identity);

        Destroy(newThunderStrike, 1f);//防止一直在不消失
    }
}