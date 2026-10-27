using UnityEngine;

public class MageBattleState : EnemyState
{
    protected Enemy_Mage enemy;
    protected Transform player;

    public MageBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Mage _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        float distanceToPlayer = Vector2.Distance(player.position, enemy.transform.position);

        // 面朝玩家
        if (player.position.x > enemy.transform.position.x && enemy.facingDir < 0)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir > 0)
            enemy.Flip();

        // 距离过近则后退
        if (distanceToPlayer < enemy.retreatDistance)
        {
            enemy.SetVelocity(-enemy.moveSpeed * enemy.facingDir, 0);
        }
        // 距离适中则停止移动
        else if (distanceToPlayer > enemy.attackDistance)
        {
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, 0);
        }
        else
        {
            enemy.SetVelocity(0, 0);
        }

        // 攻击冷却结束则攻击
        if (Time.time >= enemy.lastTimeAttack + enemy.attackCoolDown)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }
}
