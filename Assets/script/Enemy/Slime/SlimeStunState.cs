using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStunState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        stateTimer = enemy.stunDuration;

        rb.linearVelocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();


        enemy.stats.MakeInvencible(false);
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < .1f && enemy.IsGroundDetected())
        {
            enemy.fx.Invoke("CancelColorChange", 0);
            enemy.anim.SetTrigger("StunFold");
            enemy.stats.MakeInvencible(true);
        }



        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

    }
}