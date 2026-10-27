using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : EnemyState
{
    protected Enemy_Slime enemy;


    public SlimeAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();//p57攻击敌人时被设置为0速度，所以不会被击退

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}