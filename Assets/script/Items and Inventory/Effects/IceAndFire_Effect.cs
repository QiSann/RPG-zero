using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;//冰火技能预制体
    [SerializeField] private float xVelocity;//冰火技能的水平速度

    public override void ExcuteEffect(Transform _respawnPositon)
    {
        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.GetComponent<Player>().primaryAttackState.comboCounter == 2;//第三次攻击才能释放冰火技能

        if (thirdAttack)
        {

            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPositon.position, player.transform.rotation);

            newIceAndFire.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(xVelocity * player.facingDir, 0);//修改朝向

            Destroy(newIceAndFire, 10);
        }
    }
}