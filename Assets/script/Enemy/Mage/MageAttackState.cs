using UnityEngine;

public class MageAttackState : EnemyState
{
    protected Enemy_Mage enemy;
    protected Transform player;

    public MageAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Mage _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;

        // 面朝玩家
        if (player.position.x > enemy.transform.position.x && enemy.facingDir < 0)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir > 0)
            enemy.Flip();

        enemy.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            CastSpell();
            enemy.lastTimeAttack = Time.time;
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    protected virtual void CastSpell()
    {
        if (enemy.GetMagicProjectile() != null)
        {
            GameObject projectile = Object.Instantiate(enemy.GetMagicProjectile(),
                enemy.transform.position + new Vector3(enemy.facingDir * 0.5f, 0.5f, 0),
                Quaternion.identity);

            // 设置投射物方向
            MageProjectile mageProjectile = projectile.GetComponent<MageProjectile>();
            if (mageProjectile != null)
            {
                mageProjectile.SetDirection(enemy.facingDir);
            }
        }
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
}
