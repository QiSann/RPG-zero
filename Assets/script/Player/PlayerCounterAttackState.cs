using UnityEngine;


//角色弹反状态
public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        //检测指定位置和半径范围内的所有2D碰撞体（Collider2D）
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        //循环遍历colliders数组中的每一个碰撞体
        foreach (var hit in colliders)
        {
            //获取当前碰撞体hit所附加的Enemy组件,如果不为空则是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                //判断敌人是否能被弹反
                if(hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10; //设一个较大的值就行
                    player.anim.SetBool("SuccessfulCounterAttack", true);
                    if(canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.clone.CreateCloneOnCounterAttack(hit.transform);

                    }
                }
            }
        }

        if(stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
