using System.Collections;
using UnityEngine;

public class ShadyBattleState : EnemyState
{
    private Transform player;
    private Enemy_Shady enemy;
    private int moveDir;

    public float defaultSpeed;

    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        defaultSpeed = enemy.moveSpeed;

        enemy.moveSpeed = enemy.battleStateMoveSpeed;


        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

    }


    public override void Update()
    {
        base.Update();



        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
                enemy.stats.KillEntity();
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
                stateMachine.ChangeState(enemy.idleState);
        }



        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;


        if (Vector2.Distance(player.transform.position, enemy.transform.position) > 1)
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.linearVelocity.y);
        else
            enemy.SetZeroVelocity();

    }

    private void OnTriggerEnter2D(Collider2D collision)//碰到就会拾取
    {

        if (collision.GetComponent<Player>() != null)
        {
            stateMachine.ChangeState(enemy.deadState);
        }
    }
    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = defaultSpeed;
    }

}