using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; } //用于指示攻击的动画
    private float lastTimeAttacked; //最后一次攻击时间
    private float comboWindow = 2; //用于设置攻击重置时间

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0; //用于修复角色有时会向反方向攻击

        //当comboCounter>2或者超过连段时间时
        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }
        player.anim.SetInteger("ComboCounter", comboCounter);
        //这里是为了能够让角色更好的转向攻击
        float attackDir = player.facingDir;

        if(xInput != 0)
        {
            attackDir = xInput;
        }

        //这一个是用于即使在站立状态攻击时角色也会向前进行一小段距离的移动
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir , player.attackMovement[comboCounter].y);

        stateTimer = .1f;  //人物攻击时依旧有一点移动的惯性
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        comboCounter++;
        lastTimeAttacked = Time.time; //用于记录从开始到现在的时间，以秒为单位
    }

    public override void Update()
    {
        base.Update();


        if(stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
