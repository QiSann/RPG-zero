using UnityEngine;

public class PlayerDashState : PlayerState
{
    private int health;
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(SkillManager.instance.dash.dashUnlocked)
        {
            player.stats.MakeInvencible(true);
        }

        player.skill.dash.CloneOnDashStart();

        stateTimer = player.dashDuration;

    }

    public override void Exit()
    {
        base.Exit();

        player.skill.dash.CloneOnDashOver();
        player.SetVelocity(0, rb.linearVelocity.y);

        if (SkillManager.instance.dash.dashUnlocked)
        {
            player.stats.MakeInvencible(false);
        }
    }

    public override void Update()
    {
        base.Update();

        if(!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
