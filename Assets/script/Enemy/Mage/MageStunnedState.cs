using UnityEngine;

public class MageStunnedState : EnemyState
{
    protected Enemy_Mage enemy;

    public MageStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Mage _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(enemy.stunDirection.x * enemy.facingDir, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
